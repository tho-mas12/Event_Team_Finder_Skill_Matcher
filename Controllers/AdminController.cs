using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventFinder.Data;
using EventFinder.Models;

namespace EventFinder.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Admin Dashboard Overview
        public async Task<IActionResult> Index()
        {
            var totalEvents = await _context.Events.CountAsync();
            var totalPrograms = await _context.EventPrograms.CountAsync();
            var totalRegistrations = await _context.ParticipantRegistrations.CountAsync();
            var totalSeats = await _context.EventPrograms.SumAsync(p => (int?)p.MaxParticipants) ?? 0;

            var recentEvents = await _context.Events
                .Include(e => e.Programs)
                .OrderByDescending(e => e.CreatedAt)
                .Take(5)
                .ToListAsync();

            var programStats = await _context.EventPrograms
                .Include(p => p.Event)
                .Include(p => p.Registrations)
                .OrderByDescending(p => p.Registrations.Count)
                .ToListAsync();

            var recentRegistrations = await _context.ParticipantRegistrations
                .Include(r => r.EventProgram)
                    .ThenInclude(p => p!.Event)
                .OrderByDescending(r => r.RegisteredAt)
                .Take(8)
                .ToListAsync();

            var deptData = await _context.ParticipantRegistrations
                .GroupBy(r => r.Department)
                .Select(g => new { Department = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Department, x => x.Count);

            var viewModel = new AdminDashboardViewModel
            {
                TotalEvents = totalEvents,
                TotalPrograms = totalPrograms,
                TotalRegistrations = totalRegistrations,
                TotalSeatsAvailable = totalSeats - totalRegistrations,
                RecentEvents = recentEvents,
                ProgramStats = programStats,
                RecentRegistrations = recentRegistrations,
                RegistrationsByDepartment = deptData
            };

            return View(viewModel);
        }

        // Registered Event List Page (Participants Roster)
        public async Task<IActionResult> Registrations(int? programId, string? department, string? search)
        {
            var query = _context.ParticipantRegistrations
                .Include(r => r.EventProgram)
                    .ThenInclude(p => p!.Event)
                .AsQueryable();

            if (programId.HasValue && programId.Value > 0)
            {
                query = query.Where(r => r.EventProgramId == programId.Value);
            }

            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(r => r.Department == department);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.StudentName.Contains(search) || r.DeptNo.Contains(search) || r.MobileNumber.Contains(search) || r.Email.Contains(search));
            }

            var registrations = await query.OrderByDescending(r => r.RegisteredAt).ToListAsync();

            ViewBag.Programs = await _context.EventPrograms.Include(p => p.Event).ToListAsync();
            ViewBag.Departments = await _context.ParticipantRegistrations.Select(r => r.Department).Distinct().ToListAsync();
            ViewBag.SelectedProgramId = programId;
            ViewBag.SelectedDepartment = department ?? "";
            ViewBag.SearchTerm = search ?? "";

            return View(registrations);
        }

        // GET: New Events Page (Add Event Form)
        public IActionResult NewEvent()
        {
            var model = new AddEventViewModel
            {
                Programs = new List<ProgramInputModel>
                {
                    new ProgramInputModel
                    {
                        ProgramName = "Program 1",
                        Rules = "1. Follow standard conduct.\n2. Bring college ID card.",
                        MaxParticipants = 30,
                        TeamSize = 1,
                        EventDate = DateTime.Now.AddDays(7),
                        Deadline = DateTime.Now.AddDays(5)
                    }
                }
            };

            return View(model);
        }

        // POST: Create New Event & Sub-Programs
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(AddEventViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Description))
            {
                ModelState.AddModelError("", "Event Name and Description are required.");
                return View("NewEvent", model);
            }

            var newEvent = new Event
            {
                Name = model.Name,
                Description = model.Description,
                Category = model.Category,
                Location = model.Location,
                BannerGradient = model.BannerGradient,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            if (model.Programs != null && model.Programs.Count > 0)
            {
                foreach (var prog in model.Programs)
                {
                    if (!string.IsNullOrWhiteSpace(prog.ProgramName))
                    {
                        var program = new EventProgram
                        {
                            EventId = newEvent.Id,
                            ProgramName = prog.ProgramName,
                            Description = prog.Description,
                            Rules = string.IsNullOrWhiteSpace(prog.Rules) ? "1. Mandatory college ID card." : prog.Rules,
                            MaxParticipants = prog.MaxParticipants > 0 ? prog.MaxParticipants : 30,
                            TeamSize = prog.TeamSize > 0 ? prog.TeamSize : 1,
                            EventDate = prog.EventDate,
                            Deadline = prog.Deadline
                        };
                        _context.EventPrograms.Add(program);
                    }
                }
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = $"Event '{newEvent.Name}' created successfully with {newEvent.Programs.Count} program(s)!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Delete Event
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev != null)
            {
                _context.Events.Remove(ev);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Event '{ev.Name}' deleted.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Delete Registration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRegistration(int id)
        {
            var reg = await _context.ParticipantRegistrations.FindAsync(id);
            if (reg != null)
            {
                _context.ParticipantRegistrations.Remove(reg);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Registration record removed.";
            }
            return RedirectToAction(nameof(Registrations));
        }
    }
}
