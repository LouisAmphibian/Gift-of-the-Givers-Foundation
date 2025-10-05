using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Gift_of_the_Givers_Foundation.Models;
using Gift_of_the_Givers_Foundation.Data;
using Microsoft.EntityFrameworkCore;

namespace Gift_of_the_Givers_Foundation.Controllers
{
    [Authorize]
    public class DonationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Donate Resources Form
        public IActionResult Donate()
        {
            return View();
        }

        // POST: Submit Donation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Donate(Donation donation)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetInt32("UserID");
                if (userId == null)
                {
                    TempData["Error"] = "You must be logged in to make a donation.";
                    return RedirectToAction("Login", "Account");
                }

                donation.DonatedByUserID = userId.Value;
                donation.DonationDate = DateTime.UtcNow;
                donation.Status = "Pending";

                _context.Donations.Add(donation);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thank you for your donation! Our team will contact you shortly for collection.";
                return RedirectToAction("Index", "Home");
            }

            return View(donation);
        }

        // GET: List All Donations
        public async Task<IActionResult> Index()
        {
            var donations = await _context.Donations
                .Include(d => d.DonatedByUser)
                .OrderByDescending(d => d.DonationDate)
                .ToListAsync();

            return View(donations);
        }

        // GET: Donation Details
        public async Task<IActionResult> Details(int id)
        {
            var donation = await _context.Donations
                .Include(d => d.DonatedByUser)
                .FirstOrDefaultAsync(d => d.DonationID == id);

            if (donation == null)
            {
                return NotFound();
            }

            return View(donation);
        }

        // POST: Update Donation Status (Admin only)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var donation = await _context.Donations.FindAsync(id);
            if (donation == null)
            {
                return NotFound();
            }

            donation.Status = status;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Donation status updated to {status}";
            return RedirectToAction("Index");
        }
    }
}