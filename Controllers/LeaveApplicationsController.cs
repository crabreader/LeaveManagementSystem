using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LeaveManagementSystem.Data;
using LeaveManagementSystem.Models;

namespace LeaveManagementSystem.Controllers
{
    public class LeaveApplicationsController : Controller
    {
        private readonly LeaveManagementContext _context;

        public LeaveApplicationsController(LeaveManagementContext context)
        {
            _context = context;
        }

        // GET: LeaveApplications
        public async Task<IActionResult> Index(string searchString)
        {
            // Retrieve all leave applications including related employee data
            var leaveApplications = _context.LeaveApplication.Include(l => l.Employee);

            // If a search string is provided, filter leave applications by employee name
            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower(); // Convert search string to lower case

                leaveApplications = leaveApplications.Where(l => l.Employee.Name.ToLower().Contains(searchString))
                                                    .Include(l => l.Employee); // Include here
            }

            // Convert the filtered leave applications to a list and pass them to the view
            return View(await leaveApplications.ToListAsync());
        }

        // GET: LeaveApplications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveApplication = await _context.LeaveApplication
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveApplication == null)
            {
                return NotFound();
            }

            return View(leaveApplication);
        }

        // GET: LeaveApplications/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employee, "Id", "Name");
            return View();
        }

        // POST: LeaveApplications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployeeId,StartDate,EndDate,LeaveType")] LeaveApplication leaveApplication)
        {
            leaveApplication.AppliedOn = DateTime.Now;

            if (ModelState.IsValid)
            {
                // Fetch the Employee object based on the selected EmployeeId
                var employee = await _context.Employee.FindAsync(leaveApplication.EmployeeId);
                if (employee == null)
                {
                    // Handle the case where the employee is not found (optional)
                    ModelState.AddModelError(nameof(leaveApplication.EmployeeId), "Employee not found.");
                    ViewData["EmployeeId"] = new SelectList(_context.Employee, "Id", "Name");
                    return View(leaveApplication);
                }

                // Assign the Employee object to the leaveApplication.Employee property
                leaveApplication.Employee = employee;

                _context.Add(leaveApplication);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            // Fetch the list of employees
            ViewData["EmployeeId"] = new SelectList(_context.Employee, "Id", "Name");
            return View(leaveApplication);
        }

        // GET: LeaveApplications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveApplication = await _context.LeaveApplication.FindAsync(id);
            if (leaveApplication == null)
            {
                return NotFound();
            }
            return View(leaveApplication);
        }

        // POST: LeaveApplications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeId,StartDate,EndDate,LeaveType,AppliedOn")] LeaveApplication leaveApplication)
        {
            if (id != leaveApplication.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(leaveApplication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaveApplicationExists(leaveApplication.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(leaveApplication);
        }

        // GET: LeaveApplications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveApplication = await _context.LeaveApplication
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveApplication == null)
            {
                return NotFound();
            }

            return View(leaveApplication);
        }

        // POST: LeaveApplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leaveApplication = await _context.LeaveApplication.FindAsync(id);
            if (leaveApplication != null)
            {
                _context.LeaveApplication.Remove(leaveApplication);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult FilterByMonth(DateTime? month)
        {
            if (!month.HasValue)
            {
                // Default to current month if no month is provided
                month = DateTime.Today;
            }

            var filteredApplications = _context.LeaveApplication
                .Where(a => a.AppliedOn.Year == month.Value.Year && a.AppliedOn.Month == month.Value.Month)
                .Include(l => l.Employee)
                .ToList();

            return View(filteredApplications);
        }

        private bool LeaveApplicationExists(int id)
        {
            return _context.LeaveApplication.Any(e => e.Id == id);
        }
    }
}
