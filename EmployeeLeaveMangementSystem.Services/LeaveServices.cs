using EmployeeLeaveMangementSystem.Data;
using EmployeeLeaveMangementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EmployeeLeaveMangementSystem.Models.EnumDefinition;

namespace EmployeeLeaveMangementSystem.Services
{
    public class LeaveServices : ILeaveServices
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _iconfiguration;
        public LeaveServices(ApplicationDbContext context, UserManager<IdentityUser> userManager, IConfiguration iconfiguration)
        {
            _context = context; 
            _userManager = userManager;
            _iconfiguration = iconfiguration;
        }

        public async Task<bool> CreateLeave(Leave leave)
        {
            if (leave == null)
                return false;

            _context.Leaves.Add(leave);
            _context.SaveChangesAsync();
            return true;
        }

        public Leave DeleteLeave(int Id)
        {
            return _context.Leaves.Find(Id); ;
        }

        public Leave DeleteLeaveByName(int Id)
        {
            return _context.Leaves.FirstOrDefault(e => e.Id == Id);
        }

        public Leave EditLeave(int Id)
        {
            return _context.Leaves.Find(Id);
        }

        public List<Leave> GetAllLeaves()
        {
            Dictionary<string, LeaveType> enumMapping = new Dictionary<string, LeaveType>
            {
                { "SickLeave", LeaveType.SickLeave },
                { "VacationLeave", LeaveType.VacationLeave },
                { "MaternityLeave", LeaveType.MaternityLeave },
            };

            var leavesFromDb = _context.Leaves.ToList();
            var leaves = new List<Leave>();

            foreach (var leaveFromDb in leavesFromDb)
            {
                LeaveType leaveType;
                if (enumMapping.TryGetValue(leaveFromDb.LeaveType.ToString(), out leaveType))
                {
                    var leave = new Leave
                    {
                        Id = leaveFromDb.Id,
                        EmployeeId = leaveFromDb.EmployeeId,
                        Email = leaveFromDb.Email,
                        LeaveType = leaveType,
                        StartDate = leaveFromDb.StartDate,
                        EndDate = leaveFromDb.EndDate,
                        Reason = leaveFromDb.Reason,
                        Status = leaveFromDb.Status
                    };
                    leaves.Add(leave);
                }
            }

            return leaves;
        
        }

        public Task<List<Leave>> GetLeavesByEmailAsync(string email)
        {
            var employee = _context.Leaves.Where(e => e.Email == email).ToListAsync();
            return employee;
        }
    }
}
