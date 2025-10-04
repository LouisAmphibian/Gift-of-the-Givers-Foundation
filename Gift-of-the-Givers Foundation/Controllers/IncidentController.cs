using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Gift_of_the_Givers_Foundation.Models;
using Gift_of_the_Givers_Foundation.Data;
using Microsoft.EntityFrameworkCore;

namespace Gift_of_the_Givers_Foundation.Controllers
{
    [Authorize] // Only logged-in users can report incidents
    public class IncidentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IncidentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Report Incident Form
        public IActionResult Report()
        {
            return View();
        }

        // POST: Submit Incident Report
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Report(IncidentReport incidentReport)
        {
            if (ModelState.IsValid)
            {
                // Get the current logged-in user
                var userId = HttpContext.Session.GetInt32("UserID");
                if (userId == null)
                {
                    TempData["Error"] = "You must be logged in to report an incident.";
                    return RedirectToAction("Login", "Account");
                }

                incidentReport.ReportedByUserID = userId.Value;
                incidentReport.ReportedDate = DateTime.UtcNow;
                incidentReport.Status = "Reported";

                _context.IncidentReports.Add(incidentReport);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Incident reported successfully! Our team will review it shortly.";
                return RedirectToAction("Index", "Home");
            }

            // If we got this far, something failed; redisplay form
            return View(incidentReport);
        }

        // GET: List All Incidents (Admin/All users can view)
        public async Task<IActionResult> Index()
        {
            var incidents = await _context.IncidentReports
                .Include(i => i.ReportedByUser)
                .OrderByDescending(i => i.ReportedDate)
                .ToListAsync();

            return View(incidents);
        }

        // GET: Incident Details
        public async Task<IActionResult> Details(int id)
        {
            var incident = await _context.IncidentReports
                .Include(i => i.ReportedByUser)
                .FirstOrDefaultAsync(i => i.IncidentID == id);

            if (incident == null)
            {
                return NotFound();
            }

            return View(incident);
        }
    }
}