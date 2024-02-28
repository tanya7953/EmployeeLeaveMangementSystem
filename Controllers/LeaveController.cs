using EmployeeLeaveMangementSystem.Data;
using EmployeeLeaveMangementSystem.Models;
using JasperFx.CodeGeneration.Frames;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static EmployeeLeaveMangementSystem.Models.EnumDefinition;

namespace EmployeeLeaveMangementSystem.Controllers
{
    public class LeaveController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public LeaveController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
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
                        LeaveType = leaveType,
                        StartDate = leaveFromDb.StartDate,
                        EndDate = leaveFromDb.EndDate,
                        Reason = leaveFromDb.Reason,
                        Status = leaveFromDb.Status
                    };
                    leaves.Add(leave);
                }
            }

            return View(leaves);
        }





        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Leave/Create")]
        public IActionResult Create(Leave leave)
        {
            
            if (ModelState.IsValid)
            {
                
                _context.Leaves.Add(leave);
                _context.SaveChanges();
                return Redirect("/Home/Index");
            }
            return View();
        }

        public IActionResult Edit(int Id)
        {
            var employeeFromDb = _context.Leaves.Find(Id);
            if (employeeFromDb == null)
            {
                return NotFound();
            }
            return View(employeeFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        /*[Route("Leave/Edit")]*/
        /* public IActionResult Edit(Leave obj)
         {
             if (ModelState.IsValid)
             {
                 var employeeToEdit = _context.Leaves.FirstOrDefault(e => e.Id == obj.Id);

                 if (employeeToEdit == null)
                 {
                     return NotFound();
                 }
                 employeeToEdit.Status = obj.Status;

                 _context.SaveChanges();

                 TempData["success"] = "Employee Leave is edited successfully";
                 return RedirectToAction("Index");
             }
             return View(obj);
         }
 */


        public IActionResult Edit(Leave obj)
        {
            if (ModelState.IsValid)
            {
                var leaveToEdit = _context.Leaves.FirstOrDefault(e => e.Id == obj.Id);

                if (leaveToEdit == null)
                {
                    return NotFound();
                }
                leaveToEdit.Status = obj.Status;
                if (obj.Status == LeaveStatus.Approved)
                {
                    var employeeEmail = obj.Email;
                    var employee = _context.Employees.FirstOrDefault(e => e.Email == employeeEmail);
                    if (employee != null)
                    {
                        TimeSpan duration = obj.EndDate - obj.StartDate;
                        int numberOfDays = (int)duration.TotalDays;
                        switch (obj.LeaveType)
                        {
                            case LeaveType.SickLeave:
                                employee.SickLeave -= numberOfDays;
                                break;
                            case LeaveType.VacationLeave:
                                employee.VacationLeave -= numberOfDays;
                                break;
                            case LeaveType.MaternityLeave:
                                employee.MaternityLeave -= numberOfDays;
                                break;
                        }
                        _context.SaveChanges();
                    }
                }
                _context.SaveChanges();
                TempData["success"] = "Leave request is edited successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }


        public IActionResult Delete(int Id)
        {
            var employeeFromDb = _context.Leaves.Find(Id);
            if (employeeFromDb == null)
            {
                return NotFound();
            }
            return View(employeeFromDb);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public IActionResult DeleteByName(int Id)
        {
            var employeeToDelete = _context.Leaves.FirstOrDefault(e => e.Id == Id);

            if (employeeToDelete == null)
            {
                return NotFound();
            }

            _context.Leaves.Remove(employeeToDelete);
            _context.SaveChanges();
            TempData["success"] = "Employee Leave deleted successfully";
            return RedirectToAction("Index");
        }


    }
}
