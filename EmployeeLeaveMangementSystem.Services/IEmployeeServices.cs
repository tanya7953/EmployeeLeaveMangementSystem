using EmployeeLeaveMangementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeLeaveMangementSystem.Services
{
    public interface IEmployeeServices
    {
        public Employee EditEmployee(string Id);
        public Employee EditEmployee(Employee employee);
        public Employee DeleteEmployee(string Id);
        Task<bool> DeleteEmployeeByEmail(string email);
        public Employee GetEmployeeProfile(string Email);
        
    }
}
