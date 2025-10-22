using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POE_Part2_PROG6212.Models;

namespace POE_Part2_PROG6212.Controllers
{
    [Authorize(Roles = "ProgrammeCoordinator,AcademicManager")]
    public class ManagementController : Controller
    {
        private readonly IClaimStore _store;
        public ManagementController(IClaimStore store) => _store = store;

        public IActionResult ReviewClaims()
            => View(_store.GetPending());

        [HttpPost]
        public IActionResult StartReview(Guid id)
        {
            var c = _store.Get(id);
            if (c == null) { TempData["Error"] = "Claim not found."; return RedirectToAction("ReviewClaims"); }
            if (c.Status == ClaimStatus.Submitted) c.Status = ClaimStatus.UnderReview;
            TempData["Success"] = "Claim moved to Under Review.";
            return RedirectToAction("ReviewClaims");
        }

        [HttpPost]
        public IActionResult Approve(Guid id, string? comment)
        {
            if (_store.Approve(id, User.Identity?.Name ?? "Approver", comment))
                TempData["Success"] = "Claim approved.";
            else TempData["Error"] = "Claim not found.";
            return RedirectToAction("ReviewClaims");
        }

        [HttpPost]
        public IActionResult Reject(Guid id, string? comment)
        {
            if (_store.Reject(id, User.Identity?.Name ?? "Approver", comment))
                TempData["Success"] = "Claim rejected.";
            else TempData["Error"] = "Claim not found.";
            return RedirectToAction("ReviewClaims");
        }
    }
}

