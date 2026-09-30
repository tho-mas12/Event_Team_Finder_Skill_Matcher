using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventFinder.Models
{
    public class EventProgram
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        [ForeignKey("EventId")]
        public Event? Event { get; set; }

        [Required(ErrorMessage = "Program name is required")]
        [StringLength(100)]
        public string ProgramName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rules are required")]
        public string Rules { get; set; } = string.Empty;

        [Range(1, 1000, ErrorMessage = "Max participants must be at least 1")]
        public int MaxParticipants { get; set; } = 50;

        public int TeamSize { get; set; } = 1; // 1 = Individual, >1 = Team

        [DataType(DataType.DateTime)]
        public DateTime EventDate { get; set; } = DateTime.Now.AddDays(7);

        [DataType(DataType.DateTime)]
        public DateTime Deadline { get; set; } = DateTime.Now.AddDays(5);

        public List<ParticipantRegistration> Registrations { get; set; } = new List<ParticipantRegistration>();

        // Helper computed properties
        [NotMapped]
        public int RegisteredCount => Registrations?.Count ?? 0;

        [NotMapped]
        public int AvailableSeats => Math.Max(0, MaxParticipants - RegisteredCount);

        [NotMapped]
        public bool IsRegistrationOpen => DateTime.Now <= Deadline && AvailableSeats > 0;
    }
}
