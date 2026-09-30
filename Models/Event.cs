using System.ComponentModel.DataAnnotations;

namespace EventFinder.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Event name is required")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; } = "Technical"; // Technical, Cultural, Sports, Workshop, Hackathon

        public string Location { get; set; } = "Main Auditorium";

        public string BannerGradient { get; set; } = "gradient-primary"; // gradient CSS class

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        // Navigation property for programs inside this event
        public List<EventProgram> Programs { get; set; } = new List<EventProgram>();
    }
}
