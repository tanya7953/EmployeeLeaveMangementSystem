using EmployeeLeaveMangementSystem.Data;
using EmployeeLeaveMangementSystem.Models;
using EmployeeLeaveMangementSystem.Services;
using JasperFx.CodeGeneration.Frames;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using static EmployeeLeaveMangementSystem.Models.EnumDefinition;

namespace EmployeeLeaveMangementSystem.Controllers
{
    public class LeaveController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _iconfiguration;
        private readonly ILeaveServices _LeaveServices;
        public LeaveController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IConfiguration iconfiguration,ILeaveServices LeaveServices)
        {
            _context = context;
            _userManager = userManager;
            _iconfiguration = iconfiguration;
            _LeaveServices = LeaveServices;
        }

        public IActionResult Index()
        {
            var leaves = _LeaveServices.GetAllLeaves();
            return View(leaves);
        }



        public async Task<IActionResult> Status()
        {
            var user = await _userManager.GetUserAsync(User);
            var leaves = await _LeaveServices.GetLeavesByEmailAsync(user.Email);

            
            if (leaves != null)
            {
                ViewBag.leaves = leaves;

            }

            return View(leaves);
        }



        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        /* [Route("Leave/Create")]*/
        public async Task<IActionResult> Create(Leave leave)
        {
            if (ModelState.IsValid)
            {
                var result = await _LeaveServices.CreateLeave(leave);
                if (result)
                    return RedirectToAction("Status");
                TempData["success"] = "Leave Created Succesfully";
            }
            return View();
        }

        public IActionResult Edit(int Id)
        {
            var employeeFromDb = _LeaveServices.EditLeave(Id);
            if (employeeFromDb == null)
            {
                return NotFound();
            }
            return View(employeeFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Leave/Edit")]
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


        /* public IActionResult Edit(Leave obj)
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
 */


        public IActionResult Edit(Leave obj)
        {
            if (ModelState.IsValid)
            {
                var leaveToEdit = _context.Leaves.Where(e => e.Email == obj.Email).ToList();
                if (leaveToEdit == null)
                {
                    return NotFound();
                }
               /* obj.leaveToEdit = leaveToEdit;*/
                leaveToEdit.Status = obj.Status;
                TimeSpan duration = obj.EndDate - obj.StartDate;
                int numberOfDays = (int)duration.TotalDays + 1;
                var employeeId = leaveToEdit.Email;
                var employee = _context.Employees.FirstOrDefault(e => e.Email == employeeId);

                if (employee != null)
                {
                    if (obj.Status == LeaveStatus.Approved)
                    {
                       
                        switch (obj.LeaveType)
                        {  
                            case LeaveType.SickLeave:
                                employee = _context.Employees.FirstOrDefault(e => e.Email == employeeId);
                                int remainingSickLeave = employee.SickLeave - numberOfDays;
                                if (remainingSickLeave > 0)
                                {
                                    employee.SickLeave = remainingSickLeave;
                                    obj.Status = LeaveStatus.Approved;
                                    Console.WriteLine(obj.Status);
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    obj.Status = LeaveStatus.Rejected;
                                    TempData["error"] = "Insufficient sick leave balance.";
                                    _context.SaveChanges();
                                }
                                break;
                            case LeaveType.VacationLeave:
                                employee = _context.Employees.FirstOrDefault(e => e.Email == employeeId);
                                int remainingVacationLeave = employee.VacationLeave - numberOfDays;
                                if (remainingVacationLeave >= 0)
                                {
                                    employee.VacationLeave = remainingVacationLeave;
                                    obj.Status = LeaveStatus.Approved;
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    obj.Status = LeaveStatus.Rejected;
                                    TempData["error"] = "Insufficient vacation leave balance.";
                                    _context.SaveChanges();
                                }
                                break;

                            case LeaveType.MaternityLeave:
                                employee = _context.Employees.FirstOrDefault(e => e.Email == employeeId);
                                int remainingMaternityLeave = employee.MaternityLeave - numberOfDays;
                                if (remainingMaternityLeave > 0)
                                {
                                    employee.MaternityLeave = remainingMaternityLeave;
                                    obj.Status = LeaveStatus.Approved;
                                    _context.SaveChanges();

                                }
                                else
                                {
                                    obj.Status = LeaveStatus.Rejected;
                                    TempData["error"] = "Insufficient maternity leave balance.";
                                    _context.SaveChanges();
                                }
                                break;
                            default:
                                TempData["error"] = "Invalid leave type.";
                                return RedirectToAction("Index");
                                
                        }
                        _context.SaveChanges();
                        
                    }
                    else if (obj.Status == LeaveStatus.Rejected)
                    {
                        obj.Status = LeaveStatus.Rejected;
                        _context.SaveChanges();
                    }

                            
                    TempData["success"] = "Leave request is updated successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    return NotFound();
                }
            }

            return View(obj);
        }






        public IActionResult Delete(int Id)
        {
            var employeeFromDb = _LeaveServices.DeleteLeave(Id);
            if (employeeFromDb == null)
            {
                return NotFound();
            }
            TempData["success"] = "Employee Leave deleted successfully";
            return View(employeeFromDb);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public IActionResult DeleteByName(int Id)
        {
            var employeeToDelete = _LeaveServices.DeleteLeaveByName(Id);

            if (employeeToDelete == null)
            {
                return NotFound();
            }

            _context.Leaves.Remove(employeeToDelete);
            _context.SaveChanges();
            /*TempData["success"] = "Employee Leave deleted successfully";*/
            return RedirectToAction("Index");
        }


    }
}
