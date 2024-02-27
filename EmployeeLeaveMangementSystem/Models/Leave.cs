using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EmployeeLeaveMangementSystem.Models
{
    public class Leave
    {
        public int Id { get; set; }
        [Required]
        public string EmployeeId { get; set; }
        
        [Required]
        [Display(Name = "Leave Type")]
        public string LeaveType { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Reason")]
        public string Reason { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending";

    }
}
