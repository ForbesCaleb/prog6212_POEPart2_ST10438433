using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using POE_Part2_PROG6212.Models;
using System.Security.Claims;

// Alias BCL Claim to avoid conflicts with POE_Part1_PROG6212.Models.Claim
using AuthClaim = System.Security.Claims.Claim;

namespace POE_Part1_PROG6212.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserStore _userStore;
        public AuthController(IUserStore userStore) => _userStore = userStore;

        // ===== GET: Login with optional role theming =====
        [HttpGet]
        public IActionResult Login(string? returnUrl = null, string? role = null)
        {
            // Setup role-based UI theming and prefill
            role = (role ?? "").ToLowerInvariant();
            string preset = role switch
            {
                "lecturer" => "lecturer",
                "pc1" => "pc1",
                "coordinator" => "pc1",
                "am1" => "am1",
                "manager" => "am1",
                _ => ""
            };

            ViewBag.Role = role;
            ViewBag.PresetUsername = preset;
            return View(new LoginViewModel { ReturnUrl = returnUrl, Username = preset });
        }

        // ===== POST: Login Authentication =====
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var user = _userStore.Validate(model.Username, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }

            var claims = new List<AuthClaim>
            {
                new AuthClaim(ClaimTypes.NameIdentifier, user.UserId),
                new AuthClaim(ClaimTypes.Name, user.FullName),
                new AuthClaim(ClaimTypes.GivenName, user.Username),
                new AuthClaim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = model.RememberMe }
            );

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectToAction("Index", "Dashboard");
        }

        // ===== POST: Logout =====
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // ===== Denied Access =====
        public IActionResult Denied() => Content("Access denied.");
    }
}



