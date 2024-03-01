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
        private readonly ILogger<LeaveController> _logger;
        public LeaveController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IConfiguration iconfiguration, ILeaveServices LeaveServices, ILogger<LeaveController> logger)
        {
            _context = context;
            _userManager = userManager;
            _iconfiguration = iconfiguration;
            _LeaveServices = LeaveServices;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                var leaves = _LeaveServices.GetAllLeaves();
                return View(leaves);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex,"Error occured");
                return StatusCode(500);
            }
        }



        public async Task<IActionResult> Status()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var leaves = await _LeaveServices.GetLeavesByEmailAsync(user.Email);


                if (leaves != null)
                {
                    ViewBag.leaves = leaves;

                }

                return View(leaves);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching employee Leave Status.");
                return StatusCode(500);
            }
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
            try
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
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while applying leave.");
                return StatusCode(500);
            }
        }

        public IActionResult Edit(int Id)
        {
            try
            {
                var employeeFromDb = _LeaveServices.EditLeave(Id);
                if (employeeFromDb == null)
                {
                    return NotFound();
                }
                return View(employeeFromDb);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while Editing employees Leave status.");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Leave/Edit")]
        


        public IActionResult Edit(Leave obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var leaveToEdit = _context.Leaves.FirstOrDefault(e => e.EmployeeId == obj.EmployeeId && e.LeaveType == obj.LeaveType && e.StartDate == obj.StartDate && e.EndDate == obj.EndDate);
                    if (leaveToEdit == null)
                    {
                        return NotFound();
                    }
                    leaveToEdit.Status = obj.Status;
                    TimeSpan duration = obj.EndDate - obj.StartDate;
                    int numberOfDays = (int)duration.TotalDays + 1;

                    var employee = _context.Employees.FirstOrDefault(e => e.Email == obj.Email);

                    if (employee != null)
                    {
                        if (obj.Status == LeaveStatus.Approved)
                        {
                            switch (obj.LeaveType)
                            {
                                case LeaveType.SickLeave:
                                    // Deduct sick leave balance
                                    int remainingSickLeave = employee.SickLeave - numberOfDays;
                                    if (remainingSickLeave >= 0)
                                    {
                                        employee.SickLeave = remainingSickLeave;
                                    }
                                    else
                                    {
                                        obj.Status = LeaveStatus.Rejected;
                                        TempData["error"] = "Insufficient sick leave balance.";
                                    }
                                    break;
                                case LeaveType.VacationLeave:
                                    // Deduct vacation leave balance
                                    int remainingVacationLeave = employee.VacationLeave - numberOfDays;
                                    if (remainingVacationLeave >= 0)
                                    {
                                        employee.VacationLeave = remainingVacationLeave;
                                    }
                                    else
                                    {
                                        obj.Status = LeaveStatus.Rejected;
                                        TempData["error"] = "Insufficient vacation leave balance.";
                                    }
                                    break;
                                case LeaveType.MaternityLeave:
                                    // Deduct maternity leave balance
                                    int remainingMaternityLeave = employee.MaternityLeave - numberOfDays;
                                    if (remainingMaternityLeave >= 0)
                                    {
                                        employee.MaternityLeave = remainingMaternityLeave;
                                    }
                                    else
                                    {
                                        obj.Status = LeaveStatus.Rejected;
                                        TempData["error"] = "Insufficient maternity leave balance.";
                                    }
                                    break;
                                default:
                                    TempData["error"] = "Invalid leave type.";
                                    return RedirectToAction("Index");
                            }
                        }
                        else if (obj.Status == LeaveStatus.Rejected)
                        {
                            // No need to update employee balances if leave is rejected
                        }

                        _context.SaveChanges(); // Save changes after updating leave status

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return NotFound();
                    }
                }

                return View(obj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while editing employee leave status.");
                return StatusCode(500);
            }
        }



        public IActionResult Delete(int Id)
        {
            try
            {


                var employeeFromDb = _LeaveServices.DeleteLeave(Id);
                if (employeeFromDb == null)
                {
                    return NotFound();
                }
                /* TempData["success"] = "Employee Leave deleted successfully";*/
                return View(employeeFromDb);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while Deleting employee .");
                return StatusCode(500);
            }
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public IActionResult DeleteByName(int Id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting employee leave.");
                return StatusCode(500);
            }
        }


    }
}
