using EmployeeLeaveMangementSystem.Data;
using EmployeeLeaveMangementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeLeaveMangementSystem.Services
{
    public class EmployeeServices : IEmployeeServices
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _iconfiguration;

        public EmployeeServices(ApplicationDbContext db, UserManager<IdentityUser> userManager, IConfiguration iconfiguration)
        {
            _db = db;
            _userManager = userManager;
            _iconfiguration = iconfiguration;
            
        }
        public Employee GetEmployeeProfile(string Email)
        {
            
            return _db.Employees.FirstOrDefault(e => e.Email == Email);
        }
        public Employee EditEmployee(string Id)
        {
            return _db.Employees.Find(Id);
        }

        public Employee EditEmployee(Employee employee)
        {
            var employeeToEdit = _db.Employees.FirstOrDefault(e => e.Id == employee.Id);

            if (employeeToEdit == null)
            {
                return null;
            }

            employeeToEdit.FirstName = employee.FirstName;
            employeeToEdit.LastName = employee.LastName;
            employeeToEdit.PhoneNumber = employee.PhoneNumber;
            employeeToEdit.Email = employee.Email;
            employeeToEdit.Salary = employee.Salary;
            employeeToEdit.Version = Guid.NewGuid();

            _db.SaveChanges();

            return employeeToEdit;
        }

        public Employee DeleteEmployee(string Id)
        {
            return _db.Employees.Find(Id);
        }

        public async Task<bool> DeleteEmployeeByEmail(string email)
        {
            var employeeToDelete = await _db.Employees.FirstOrDefaultAsync(e => e.Email == email);

            if (employeeToDelete == null)
            {
                return false;
            }

            var leavesToDelete = _db.Leaves.Where(l => l.Email == email);
            _db.Leaves.RemoveRange(leavesToDelete);
            _db.Employees.Remove(employeeToDelete);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}
