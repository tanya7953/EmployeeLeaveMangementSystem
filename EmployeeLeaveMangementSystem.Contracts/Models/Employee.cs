using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EmployeeLeaveMangementSystem.Models
{
    public class Employee 
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        public int Salary { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        public string Email { get; set; }

        public int SickLeave { get; set; } 
        public int VacationLeave { get; set; } 
        public int MaternityLeave { get; set; } 



        [ConcurrencyCheck]
        public Guid Version { get; set; }
    }
}
