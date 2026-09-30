namespace EventFinder.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalEvents { get; set; }
        public int TotalPrograms { get; set; }
        public int TotalRegistrations { get; set; }
        public int TotalSeatsAvailable { get; set; }

        public List<Event> RecentEvents { get; set; } = new List<Event>();
        public List<EventProgram> ProgramStats { get; set; } = new List<EventProgram>();
        public List<ParticipantRegistration> RecentRegistrations { get; set; } = new List<ParticipantRegistration>();

        public Dictionary<string, int> RegistrationsByDepartment { get; set; } = new Dictionary<string, int>();
    }

    public class AddEventViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = "Technical";
        public string Location { get; set; } = "Main Campus Auditorium";
        public string BannerGradient { get; set; } = "gradient-primary";

        // Programs list attached during event creation
        public List<ProgramInputModel> Programs { get; set; } = new List<ProgramInputModel>();
    }

    public class ProgramInputModel
    {
        public string ProgramName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Rules { get; set; } = string.Empty;
        public int MaxParticipants { get; set; } = 30;
        public int TeamSize { get; set; } = 1;
        public DateTime EventDate { get; set; } = DateTime.Now.AddDays(7);
        public DateTime Deadline { get; set; } = DateTime.Now.AddDays(5);
    }

    public class TeamFinderViewModel
    {
        public int? SelectedProgramId { get; set; }
        public string SelectedSkill { get; set; } = string.Empty;
        public string SearchTerm { get; set; } = string.Empty;

        public List<EventProgram> AvailablePrograms { get; set; } = new List<EventProgram>();
        public List<ParticipantRegistration> ParticipantsLookingForTeam { get; set; } = new List<ParticipantRegistration>();
        public List<string> PopularSkills { get; set; } = new List<string>();
    }
}
