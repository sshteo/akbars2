using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using akbars.Extensions;
using akbars.Services;
using akbars.ViewModels;

namespace akbars.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILookupService _lookupService;
        private readonly ITicketService _ticketService;

        public AdminController(
            IUserService userService,
            ILookupService lookupService,
            ITicketService ticketService)
        {
            _userService = userService;
            _lookupService = lookupService;
            _ticketService = ticketService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var session = User.ToSessionContext();
            if (session == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(new AdminDashboardViewModel
            {
                Session = session,
                Stats = _ticketService.GetStatistics(null, null),
                Users = _userService.GetUsers(null),
                Roles = _lookupService.GetRoles(),
                Priorities = _lookupService.GetPriorities(),
                Statuses = _lookupService.GetStatuses(),
                TicketTypes = _lookupService.GetTicketTypes()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateRole(UpdateRoleForm form)
        {
            _userService.UpdateRole(form.UserId, form.RoleId);
            TempData["Success"] = "Роль пользователя обновлена.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPriority(AddPriorityForm form)
        {
            _lookupService.AddPriority(form.Name.Trim(), form.SlaHours);
            TempData["Success"] = "Приоритет добавлен.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddStatus(AddStatusForm form)
        {
            _lookupService.AddStatus(form.Name.Trim(), form.Description.Trim());
            TempData["Success"] = "Статус добавлен.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddType(AddTypeForm form)
        {
            _lookupService.AddTicketType(form.Name.Trim());
            TempData["Success"] = "Тип заявки добавлен.";
            return RedirectToAction(nameof(Index));
        }
    }
}
