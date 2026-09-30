using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventFinder.Data;
using EventFinder.Models;

namespace EventFinder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Public Event Catalogue / User Dashboard
        public async Task<IActionResult> Index(string? category, string? search)
        {
            var query = _context.Events
                .Include(e => e.Programs)
                    .ThenInclude(p => p.Registrations)
                .Where(e => e.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(e => e.Category.ToLower() == category.ToLower());
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.Name.Contains(search) || e.Description.Contains(search));
            }

            var events = await query.OrderByDescending(e => e.CreatedAt).ToListAsync();

            ViewBag.CurrentCategory = category ?? "All";
            ViewBag.SearchTerm = search ?? "";

            return View(events);
        }

        // View Event Program Details, Rules, and Registration Modal
        public async Task<IActionResult> ProgramDetails(int id)
        {
            var program = await _context.EventPrograms
                .Include(p => p.Event)
                .Include(p => p.Registrations)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (program == null)
            {
                TempData["ErrorMessage"] = "Event Program not found!";
                return RedirectToAction(nameof(Index));
            }

            return View(program);
        }

        // POST: Register for Event Program
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(ParticipantRegistration registration)
        {
            var program = await _context.EventPrograms
                .Include(p => p.Registrations)
                .FirstOrDefaultAsync(p => p.Id == registration.EventProgramId);

            if (program == null)
            {
                TempData["ErrorMessage"] = "Invalid event program selection.";
                return RedirectToAction(nameof(Index));
            }

            // Deadline check
            if (DateTime.Now > program.Deadline)
            {
                TempData["ErrorMessage"] = "Registration deadline for this program has expired!";
                return RedirectToAction(nameof(ProgramDetails), new { id = program.Id });
            }

            // Max capacity check
            if (program.Registrations.Count >= program.MaxParticipants)
            {
                TempData["ErrorMessage"] = "Sorry, this event program has reached its maximum participant limit!";
                return RedirectToAction(nameof(ProgramDetails), new { id = program.Id });
            }

            // Duplicate registration check for same DeptNo in same program
            bool alreadyRegistered = await _context.ParticipantRegistrations
                .AnyAsync(r => r.EventProgramId == registration.EventProgramId && r.DeptNo.ToLower() == registration.DeptNo.ToLower());

            if (alreadyRegistered)
            {
                TempData["ErrorMessage"] = $"Register Number '{registration.DeptNo}' is already registered for this event program!";
                return RedirectToAction(nameof(ProgramDetails), new { id = program.Id });
            }

            registration.RegisteredAt = DateTime.Now;
            _context.ParticipantRegistrations.Add(registration);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Congratulations {registration.StudentName}! You have successfully registered for '{program.ProgramName}'.";
            return RedirectToAction(nameof(ProgramDetails), new { id = program.Id });
        }

        // Skill Matcher & Team Finder Dashboard for Students
        public async Task<IActionResult> TeamFinder(int? programId, string? skill, string? search)
        {
            var programs = await _context.EventPrograms
                .Include(p => p.Event)
                .Where(p => p.TeamSize > 1)
                .ToListAsync();

            var query = _context.ParticipantRegistrations
                .Include(r => r.EventProgram)
                    .ThenInclude(p => p!.Event)
                .Where(r => r.LookingForTeam == "Yes")
                .AsQueryable();

            if (programId.HasValue && programId.Value > 0)
            {
                query = query.Where(r => r.EventProgramId == programId.Value);
            }

            if (!string.IsNullOrEmpty(skill))
            {
                query = query.Where(r => r.Skills.ToLower().Contains(skill.ToLower()));
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.StudentName.Contains(search) || r.Department.Contains(search) || r.DeptNo.Contains(search));
            }

            var participants = await query.OrderByDescending(r => r.RegisteredAt).ToListAsync();

            // Extract popular skills
            var allSkills = await _context.ParticipantRegistrations
                .Where(r => !string.IsNullOrEmpty(r.Skills))
                .Select(r => r.Skills)
                .ToListAsync();

            var topSkills = allSkills
                .SelectMany(s => s.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(s => s.Trim())
                .GroupBy(s => s, StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Take(10)
                .ToList();

            var viewModel = new TeamFinderViewModel
            {
                SelectedProgramId = programId,
                SelectedSkill = skill ?? "",
                SearchTerm = search ?? "",
                AvailablePrograms = programs,
                ParticipantsLookingForTeam = participants,
                PopularSkills = topSkills
            };

            return View(viewModel);
        }
    }
}
