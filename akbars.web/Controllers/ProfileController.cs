using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using akbars.Extensions;
using akbars.Models;
using akbars.Services;
using akbars.ViewModels;

namespace akbars.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;

        public ProfileController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Edit()
        {
            var session = User.ToSessionContext();
            if (session == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _userService.GetUser(session.UserId);
            if (user == null)
            {
                TempData["Error"] = "Пользователь не найден.";
                return RedirectToAction("Index", "Dashboard");
            }

            return View(new ProfileEditViewModel
            {
                Id = user.Id,
                LastName = user.LastName ?? string.Empty,
                FirstName = user.FirstName ?? string.Empty,
                MiddleName = user.MiddleName ?? string.Empty,
                Department = user.Department ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Phone = user.Phone ?? string.Empty
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProfileEditViewModel model)
        {
            var session = User.ToSessionContext();
            if (session == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _userService.UpdateProfile(new User
            {
                Id = session.UserId,
                LastName = model.LastName.Trim(),
                FirstName = model.FirstName.Trim(),
                MiddleName = model.MiddleName.Trim(),
                Department = model.Department.Trim(),
                Email = model.Email.Trim(),
                Phone = model.Phone.Trim()
            });

            TempData["Success"] = "Профиль обновлен.";
            return RedirectToAction(nameof(Edit));
        }
    }
}
