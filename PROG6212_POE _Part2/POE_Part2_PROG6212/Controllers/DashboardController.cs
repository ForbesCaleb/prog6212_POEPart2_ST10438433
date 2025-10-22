using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POE_Part2_PROG6212.Models;
using System.Security.Claims;

namespace POE_Part2_PROG6212.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IClaimStore _claimStore;
        private readonly IFileStorage _files;

        public DashboardController(IClaimStore store, IFileStorage files)
        {
            _claimStore = store;
            _files = files;
        }

        // === DASHBOARD ===
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "";
            if (role == "Lecturer")
                ViewBag.RecentClaims = _claimStore.GetByUser(userId);
            return View();
        }

        // === LECTURER: SUBMIT CLAIM ===
        [Authorize(Roles = "Lecturer")]
        [HttpGet]
        public IActionResult SubmitClaim() => View(new SubmitClaimViewModel());

        [Authorize(Roles = "Lecturer")]
        [HttpPost]
        public async Task<IActionResult> SubmitClaim(SubmitClaimViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var name = User.Identity?.Name ?? "Lecturer";

            var claim = new ClaimDto
            {
                UserId = userId,
                LecturerName = name,
                DateWorked = model.DateWorked,
                HoursWorked = model.HoursWorked,
                Activity = model.Activity,
                HourlyRate = model.HourlyRate,
                TotalAmount = model.TotalAmount,
                Notes = model.Notes,
                Status = ClaimStatus.Submitted
            };
            _claimStore.Add(claim);

            if (model.Document != null)
            {
                try
                {
                    var doc = await _files.SaveAsync(model.Document);
                    _claimStore.AddDocument(claim.ClaimId, doc);
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Upload failed: {ex.Message}";
                }
            }

            TempData["Success"] = $"Claim submitted successfully (R{model.TotalAmount:N2}).";
            return RedirectToAction("Index");
        }

        // === LECTURER: MY CLAIMS LIST ===
        [Authorize(Roles = "Lecturer")]
        public IActionResult MyClaims()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var claims = _claimStore.GetByUser(userId, 50);
            return View(claims);
        }

        // === NEW: JSON status feed for live updates ===
        [Authorize(Roles = "Lecturer")]
        [HttpGet]
        public IActionResult MyClaimsStatusData()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var data = _claimStore.GetByUser(userId, 50)
                                  .Select(c => new { id = c.ClaimId, status = c.Status.ToString() });
            return Json(data);
        }
    }
}
