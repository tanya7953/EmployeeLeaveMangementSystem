using EmployeeLeaveMangementSystem.Data;
using EmployeeLeaveMangementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeLeaveMangementSystem.Controllers
{
    public class LeaveController : Controller
    {

        private readonly ApplicationDbContext _context;

        public LeaveController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var leaves = _context.Leaves.ToList();
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
        public IActionResult Edit(Leave obj)
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
