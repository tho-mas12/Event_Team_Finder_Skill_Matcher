using Microsoft.EntityFrameworkCore;
using EventFinder.Models;

namespace EventFinder.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<EventProgram> EventPrograms { get; set; } = null!;
        public DbSet<ParticipantRegistration> ParticipantRegistrations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Event>()
                .HasMany(e => e.Programs)
                .WithOne(p => p.Event)
                .HasForeignKey(p => p.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventProgram>()
                .HasMany(p => p.Registrations)
                .WithOne(r => r.EventProgram)
                .HasForeignKey(r => r.EventProgramId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public static void SeedData(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Events.Any())
            {
                var event1 = new Event
                {
                    Name = "TechXplore 2026",
                    Description = "Annual National Level Technical Symposium featuring coding marathons, web design, paper presentations, and robotics.",
                    Category = "Technical",
                    Location = "Tech Block Auditorium",
                    BannerGradient = "gradient-indigo",
                    CreatedAt = DateTime.Now.AddDays(-2),
                    IsActive = true
                };

                var event2 = new Event
                {
                    Name = "InnovateX Hackathon",
                    Description = "24-Hour continuous hackathon to build solutions for real-world sustainability, AI, and healthcare challenges.",
                    Category = "Hackathon",
                    Location = "Innovation Lab 3",
                    BannerGradient = "gradient-purple",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    IsActive = true
                };

                var event3 = new Event
                {
                    Name = "Campus Culturals 2026",
                    Description = "Inter-departmental cultural festival with music, dance, dramatic arts, and photography competitions.",
                    Category = "Cultural",
                    Location = "Open Air Theatre",
                    BannerGradient = "gradient-pink",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                context.Events.AddRange(event1, event2, event3);
                context.SaveChanges();

                var program1 = new EventProgram
                {
                    EventId = event1.Id,
                    ProgramName = "Code Golf & Speed Coding",
                    Description = "Write the shortest, fastest C# or Python solutions for algorithmic puzzles.",
                    Rules = "1. Individual participation only.\n2. Standard languages allowed: C++, C#, Python, Java.\n3. Internet access prohibited during round 2.\n4. Decision of judges is final.",
                    MaxParticipants = 40,
                    TeamSize = 1,
                    EventDate = DateTime.Now.AddDays(8),
                    Deadline = DateTime.Now.AddDays(5)
                };

                var program2 = new EventProgram
                {
                    EventId = event1.Id,
                    ProgramName = "UI/UX Web Makeover",
                    Description = "Redesign a legacy university dashboard into a sleek responsive website within 3 hours.",
                    Rules = "1. Teams of up to 2 members.\n2. Must use Figma or HTML/CSS/Tailwind.\n3. Original designs required; plagiarism leads to disqualification.",
                    MaxParticipants = 25,
                    TeamSize = 2,
                    EventDate = DateTime.Now.AddDays(8),
                    Deadline = DateTime.Now.AddDays(6)
                };

                var program3 = new EventProgram
                {
                    EventId = event2.Id,
                    ProgramName = "AI & Smart Tech Prototype",
                    Description = "Build a working prototype using OpenAI APIs, IoT, or Machine Learning models.",
                    Rules = "1. Team size: 2 to 4 members.\n2. Mentors will evaluate at midnight and 8 AM.\n3. Code repository must be submitted via GitHub link.",
                    MaxParticipants = 20,
                    TeamSize = 4,
                    EventDate = DateTime.Now.AddDays(12),
                    Deadline = DateTime.Now.AddDays(9)
                };

                var program4 = new EventProgram
                {
                    EventId = event3.Id,
                    ProgramName = "Battle of the Bands",
                    Description = "Live rock, pop, and acoustic band competition.",
                    Rules = "1. 15 minutes setup + performance time limit.\n2. Maximum 6 members per band.\n3. Drums and PA provided.",
                    MaxParticipants = 10,
                    TeamSize = 5,
                    EventDate = DateTime.Now.AddDays(15),
                    Deadline = DateTime.Now.AddDays(10)
                };

                context.EventPrograms.AddRange(program1, program2, program3, program4);
                context.SaveChanges();

                // Seed sample registrations
                var reg1 = new ParticipantRegistration
                {
                    EventProgramId = program1.Id,
                    StudentName = "Alex Rivera",
                    DeptNo = "21CS045",
                    Department = "CSE",
                    MobileNumber = "9876543210",
                    Email = "alex.rivera@college.edu",
                    Skills = "C#, Python, Algorithms",
                    LookingForTeam = "No",
                    RegisteredAt = DateTime.Now.AddDays(-1)
                };

                var reg2 = new ParticipantRegistration
                {
                    EventProgramId = program2.Id,
                    StudentName = "Sophia Chen",
                    DeptNo = "22IT102",
                    Department = "IT",
                    MobileNumber = "9812345678",
                    Email = "sophia.chen@college.edu",
                    Skills = "Figma, CSS, Tailwind",
                    LookingForTeam = "Yes",
                    RegisteredAt = DateTime.Now.AddHours(-10)
                };

                var reg3 = new ParticipantRegistration
                {
                    EventProgramId = program3.Id,
                    StudentName = "Rohan Sharma",
                    DeptNo = "21EC089",
                    Department = "ECE",
                    MobileNumber = "9988776655",
                    Email = "rohan.s@college.edu",
                    Skills = "Python, PyTorch, Node.js",
                    LookingForTeam = "Yes",
                    RegisteredAt = DateTime.Now.AddHours(-4)
                };

                context.ParticipantRegistrations.AddRange(reg1, reg2, reg3);
                context.SaveChanges();
            }
        }
    }
}
