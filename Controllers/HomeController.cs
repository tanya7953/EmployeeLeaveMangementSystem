using EmployeeLeaveMangementSystem.Data;
using EmployeeLeaveMangementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static System.Net.WebRequestMethods;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace EmployeeLeaveMangementSystem.Controllers
{
   
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _iconfiguration;


        //private readonly ILogger<CategoryController> _logger;
        public HomeController(ApplicationDbContext db, UserManager<IdentityUser> userManager,IConfiguration iconfiguration, ILogger<HomeController> logger)
        {
            _db = db;
            _userManager = userManager;
            _iconfiguration = iconfiguration;
            _logger = logger;
        }

        public IActionResult Index()
        {

            
            return View();
        }
        public IActionResult Details()
        {
            IEnumerable<Employee> objEmployeeList = _db.Employees;
            return View(objEmployeeList);
        }

        public IActionResult Profile()
        {

            var sickLeave = _iconfiguration.GetValue<int>("SickLeave");
            var vacationLeave = _iconfiguration.GetValue<int>("VacationLeave");
            var maternityLeave = _iconfiguration.GetValue<int>("MaternityLeave");

            var user = _userManager.GetUserAsync(User).Result;
            var employee = _db.Employees.FirstOrDefault(e => e.Email == user.Email);

            if (employee != null)
            {
                ViewBag.Email = employee.Email;
                ViewBag.FirstName = employee.FirstName;
                ViewBag.LastName = employee.LastName;
                ViewBag.Salary =employee.Salary;
                ViewBag.PhoneNumber = employee.PhoneNumber;
                ViewBag.SickLeave = employee.SickLeave;
                ViewBag.VacationLeave = employee.VacationLeave;
                ViewBag.MaternityLeave = employee.MaternityLeave;
            }

            return View();
        }


        //GET
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        /* public IActionResult Create(Employee obj)
         {

             if (ModelState.IsValid)
             {
                 _db.Employees.Add(obj);
                 _db.SaveChanges();
                 TempData["success"] = "Employee Added successfully";
                 return RedirectToAction("Details");
             }
             return View();
         }*/




        public async Task<IActionResult> Create(Employee obj)
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
         

        public IActionResult Edit(string Id)
        {
            var employeeFromDb = _db.Employees.Find(Id);
            if (employeeFromDb == null)
            {
                return NotFound();
            }
            return View(employeeFromDb);
        }
        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Employee obj)
        {
            if (ModelState.IsValid)
            {
                var employeeToEdit = _db.Employees.FirstOrDefault(e => e.Id == obj.Id);

                if (employeeToEdit == null)
                {
                    return NotFound();
                }
                employeeToEdit.FirstName = obj.FirstName;
                employeeToEdit.LastName = obj.LastName;
               
                employeeToEdit.PhoneNumber = obj.PhoneNumber;
                employeeToEdit.Email = obj.Email;
                employeeToEdit.Salary = obj.Salary;
                employeeToEdit.Version = Guid.NewGuid();
                _db.SaveChanges();

                TempData["success"] = "Employee Details is edited successfully";
                return RedirectToAction("Details");
            }
            return View(obj);
        }


        public IActionResult Delete(string Id)
        {
            var employeeFromDb = _db.Employees.Find(Id);
            if (employeeFromDb == null)
            {
                return NotFound();
            }
            return View(employeeFromDb);
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
        public IActionResult DeleteByName(string Email)
        {
            var employeeToDelete = _db.Employees.FirstOrDefault(e => e.Email == Email);
            if (employeeToDelete == null)
            {
                return NotFound();
            }
            var leavesToDelete = _db.Leaves.Where(l => l.Email == Email);
            _db.Leaves.RemoveRange(leavesToDelete);
            _db.Employees.Remove(employeeToDelete);
            _db.SaveChanges();
            TempData["success"] = "Employee and associated leave entries deleted successfully";
            return RedirectToAction("Details");
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