using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EmployeeLeaveMangementSystem.Models
{
    public class Employee:IdentityUser
    {

        
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string UserName {  get; set; }
        public int Salary { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        public string Email { get; set; }

        [ConcurrencyCheck]
        public Guid Version { get; set; }




    }
}
