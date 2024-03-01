using EmployeeLeaveMangementSystem.Data;
using EmployeeLeaveMangementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static System.Net.WebRequestMethods;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using EmployeeLeaveMangementSystem.Services;
using Serilog;
namespace EmployeeLeaveMangementSystem.Controllers
{
   
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _iconfiguration;
        private readonly IEmployeeServices _EmployeeServices;



        public HomeController(ApplicationDbContext db, UserManager<IdentityUser> userManager,IConfiguration iconfiguration, ILogger<HomeController> logger,IEmployeeServices EmployeeServices)
        {
            _db = db;
            _userManager = userManager;
            _iconfiguration = iconfiguration;
            _logger = logger;
            _EmployeeServices = EmployeeServices;

        }

        public IActionResult Index()
        {
            _logger.LogInformation("Leave Management System Executing..");

            return View();
        }
        public IActionResult Details()
        {
            try
            {
                IEnumerable<Employee> objEmployeeList = _db.Employees;
                _logger.LogInformation("Employee Details =>{@objEmployeeList}", objEmployeeList);
                return View(objEmployeeList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching employee details.");
                return StatusCode(500);
            }
        }

        public IActionResult Profile()
        {
            try
            {
                var user = _userManager.GetUserAsync(User).Result;
                var employee = _EmployeeServices.GetEmployeeProfile(user.Email);

                if (employee != null)
                {
                    ViewBag.Email = employee.Email;
                    ViewBag.FirstName = employee.FirstName;
                    ViewBag.LastName = employee.LastName;
                    ViewBag.Salary = employee.Salary;
                    ViewBag.PhoneNumber = employee.PhoneNumber;
                    ViewBag.SickLeave = employee.SickLeave;
                    ViewBag.VacationLeave = employee.VacationLeave;
                    ViewBag.MaternityLeave = employee.MaternityLeave;
                }
                _logger.LogInformation("Profile of Employee");

                return View();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching employee Profile.");
                return StatusCode(500);
            }
        }


        //GET
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
       



        public async Task<IActionResult> Create(Employee obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var sickLeave = _iconfiguration["SickLeave"];
                    var vacationLeave = _iconfiguration["VacationLeave"];
                    var maternityLeave = _iconfiguration["MaternityLeave"];


                    var user = new IdentityUser
                    {
                        Email = obj.Email,
                        PhoneNumber = obj.PhoneNumber,
                        UserName = obj.Email

                    };
                    var result = await _userManager.CreateAsync(user, "Employee@12");
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Employee");
                        obj.SickLeave = int.Parse(sickLeave);
                        obj.VacationLeave = int.Parse(vacationLeave);
                        obj.MaternityLeave = int.Parse(maternityLeave);
                        _db.Employees.Add(obj);
                        _db.SaveChanges();
                        TempData["success"] = "Employee Added successfully";
                        return RedirectToAction("Details");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                return View(obj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while Creating employee details.");
                return StatusCode(500);
            }
        }
         

        public IActionResult Edit(string Id)
        {
            try
            {
                var employeeFromDb = _EmployeeServices.EditEmployee(Id);
                if (employeeFromDb == null)
                {
                    return NotFound();
                }

                return View(employeeFromDb);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while editing employee details.");
                return StatusCode(500);
            }
        }
        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Employee obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var editedEmployee = _EmployeeServices.EditEmployee(obj);
                    if (editedEmployee == null)
                    {
                        return NotFound();
                    }
                    TempData["success"] = "Employee Details have been edited successfully";
                    return RedirectToAction("Details");
                }

                return View(obj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while Editing employee details.");
                return StatusCode(500);
            }
        }


        public IActionResult Delete(string Id)
        {
            try
            {
                var employeeFromDb = _EmployeeServices.DeleteEmployee(Id);
                if (employeeFromDb == null)
                {
                    return NotFound();
                }
                return View(employeeFromDb);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while Deleting employee details.");
                return StatusCode(500);
            }
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        /* public IActionResult DeleteByName(string Id)
         {
             var employeeToDelete = _db.Employees.FirstOrDefault(e => e.Id == Id);

             if (employeeToDelete == null)
             {
                 return NotFound();
             }

             _db.Employees.Remove(employeeToDelete);
             _db.SaveChanges();
             TempData["success"] = "Employee deleted successfully";
             return RedirectToAction("Details");
         }*/

        public async Task<IActionResult> DeleteByName(string email)
        {
            try
            {
                var success = await _EmployeeServices.DeleteEmployeeByEmail(email);
                if (success)
                {
                    TempData["success"] = "Employee and associated leave entries deleted successfully";
                }
                else
                {
                    TempData["error"] = "Employee not found";
                }
                return RedirectToAction("Details");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while Deleting employee details.");
                return StatusCode(500);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}