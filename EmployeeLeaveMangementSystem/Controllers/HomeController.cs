using EmployeeLeaveMangementSystem.Data;
using EmployeeLeaveMangementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static System.Net.WebRequestMethods;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace EmployeeLeaveMangementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext _db;
        //rivate readonly ILogger<CategoryController> _logger;
        public HomeController(ApplicationDbContext db)
        {
            _db = db;
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
        //GET
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Employee obj)
        {
           
            if (ModelState.IsValid)
            {
                _db.Employees.Add(obj);
                _db.SaveChanges();
                TempData["success"] = "Employee Added successfully";
                return RedirectToAction("Details");
            }
            return View();
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
                employeeToEdit.UserName = obj.UserName;
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
        
        public IActionResult DeleteByName(string Id)
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
