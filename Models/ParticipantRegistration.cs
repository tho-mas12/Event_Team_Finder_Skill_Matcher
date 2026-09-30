using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventFinder.Models
{
    public class ParticipantRegistration
    {
        public int Id { get; set; }

        public int EventProgramId { get; set; }

        [ForeignKey("EventProgramId")]
        public EventProgram? EventProgram { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100)]
        public string StudentName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department / Register Number is required")]
        [StringLength(50)]
        public string DeptNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department is required")]
        public string Department { get; set; } = string.Empty; // e.g. CSE, IT, ECE, EEE, Mech, MBA

        [Required(ErrorMessage = "Mobile Number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(15)]
        public string MobileNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Skills / Expertise")]
        public string Skills { get; set; } = string.Empty; // e.g. "C#, React, UI Design"

        public string LookingForTeam { get; set; } = "Yes"; // "Yes" or "No"

        public DateTime RegisteredAt { get; set; } = DateTime.Now;
    }
}
