using EmployeeLeaveMangementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeLeaveMangementSystem.Services
{
    public interface ILeaveServices
    {
        public List<Leave> GetAllLeaves();
        Task<List<Leave>> GetLeavesByEmailAsync(string email);
        Task<bool> CreateLeave(Leave leave);
        public Leave EditLeave(int Id);

        public Leave DeleteLeave(int Id);
        public Leave DeleteLeaveByName(int Id);
    }
}
