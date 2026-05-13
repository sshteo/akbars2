using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using akbars.Extensions;
using akbars.Services;
using akbars.ViewModels;

namespace akbars.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            var model = BuildLoginViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var statusModel = BuildLoginViewModel();
            model.SystemAvailable = statusModel.SystemAvailable;
            model.SystemStatus = statusModel.SystemStatus;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!model.SystemAvailable)
            {
                model.ErrorMessage = "Система недоступна. Сначала восстановите подключение к базе.";
                return View(model);
            }

            var result = _authService.Authenticate(model.Login, model.Password);
            if (!result.Success || result.Session == null)
            {
                model.ErrorMessage = result.ErrorMessage;
                return View(model);
            }

            var session = result.Session;
            var appRole = RoleNames.FromRoleId(session.RoleId);
            if (string.IsNullOrWhiteSpace(appRole))
            {
                model.ErrorMessage = "Для этой роли веб-интерфейс пока не настроен.";
                return View(model);
            }

            var claims = new List<Claim>
            {
                new("user_id", session.UserId.ToString()),
                new("full_name", session.FullName ?? string.Empty),
                new("first_name", session.FirstName ?? string.Empty),
                new("last_name", session.LastName ?? string.Empty),
                new("middle_name", session.MiddleName ?? string.Empty),
                new("email", session.Email ?? string.Empty),
                new("phone", session.Phone ?? string.Empty),
                new("department", session.Department ?? string.Empty),
                new("role_id", session.RoleId.ToString()),
                new("role_name", session.RoleName ?? string.Empty),
                new(ClaimTypes.Name, session.FullName ?? session.LoginFallback()),
                new(ClaimTypes.Role, appRole)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        private LoginViewModel BuildLoginViewModel()
        {
            string errorMessage;
            var systemAvailable = _authService.CanConnect(out errorMessage);

            return new LoginViewModel
            {
                SystemAvailable = systemAvailable,
                SystemStatus = systemAvailable
                    ? "База данных доступна. Можно входить в систему."
                    : errorMessage
            };
        }
    }

    internal static class SessionContextFallbackExtensions
    {
        public static string LoginFallback(this akbars.Models.SessionContext session)
        {
            return string.IsNullOrWhiteSpace(session.FirstName)
                ? "user"
                : session.FirstName;
        }
    }
}
