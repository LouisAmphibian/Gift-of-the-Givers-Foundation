using System.Security.Claims;
using Gift_of_the_Givers_Foundation.Models;
using Gift_of_the_Givers_Foundation.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Gift_of_the_Givers_Foundation.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                var user = await _userService.AuthenticateUserAsync(email, password);
                if (user != null)
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role)
            };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    HttpContext.Session.SetInt32("UserID", user.UserID);
                    HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");
                    HttpContext.Session.SetString("UserRole", user.Role);

                    TempData["Success"] = "Login successful!";
                    return RedirectToAction("Index", "Home");
                }

                TempData["Error"] = "Invalid email or password";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Login failed: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string firstName, string lastName, string email, string password, string confirmPassword, string role)
        {
            try
            {
                if (password != confirmPassword)
                {
                    TempData["Error"] = "Passwords do not match";
                    return RedirectToAction("Index", "Home");
                }

                if (await _userService.UserExistsAsync(email))
                {
                    TempData["Error"] = "User with this email already exists";
                    return RedirectToAction("Index", "Home");
                }

                var user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Role = role ?? "Donor"
                };

                var newUser = await _userService.RegisterUserAsync(user, password);

                // Auto-login after registration
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, newUser.UserID.ToString()),
                    new Claim(ClaimTypes.Email, newUser.Email),
                    new Claim(ClaimTypes.Name, $"{newUser.FirstName} {newUser.LastName}"),
                    new Claim(ClaimTypes.Role, newUser.Role)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                HttpContext.Session.SetInt32("UserID", newUser.UserID);
                HttpContext.Session.SetString("UserName", $"{newUser.FirstName} {newUser.LastName}");
                HttpContext.Session.SetString("UserRole", newUser.Role);

                TempData["Success"] = "Registration successful!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Registration failed: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

    }
}