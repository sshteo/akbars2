using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using akbars.Extensions;
using akbars.Models;
using akbars.Services;
using akbars.ViewModels;

namespace akbars.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly IUserService _userService;

        public DashboardController(ITicketService ticketService, IUserService userService)
        {
            _ticketService = ticketService;
            _userService = userService;
        }

        public IActionResult Index()
        {
            var session = RequireSession();
            if (session == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var appRole = RoleNames.FromRoleId(session.RoleId);
            return appRole switch
            {
                RoleNames.Employee => RedirectToAction(nameof(Employee)),
                RoleNames.Executor => RedirectToAction(nameof(Executor)),
                RoleNames.Dispatcher => RedirectToAction(nameof(Dispatcher)),
                RoleNames.Admin => RedirectToAction(nameof(Admin)),
                _ => RedirectToAction("Login", "Account")
            };
        }

        [Authorize(Roles = RoleNames.Employee)]
        public IActionResult Employee()
        {
            var session = RequireSession()!;
            return View(new EmployeeDashboardViewModel
            {
                Session = session,
                Stats = _ticketService.GetStatistics(session.UserId, null)
            });
        }

        [Authorize(Roles = RoleNames.Executor)]
        public IActionResult Executor()
        {
            var session = RequireSession()!;
            return View(new ExecutorDashboardViewModel
            {
                Session = session,
                Stats = _ticketService.GetStatistics(null, session.UserId),
                Tickets = _ticketService.GetTickets(new TicketQuery { AssigneeId = session.UserId })
            });
        }

        [Authorize(Roles = RoleNames.Dispatcher)]
        public IActionResult Dispatcher()
        {
            var session = RequireSession()!;
            return View(new DispatcherDashboardViewModel
            {
                Session = session,
                Stats = _ticketService.GetStatistics(null, null),
                Employees = _userService.GetUsers(1).OrderBy(item => item.LastName).ToList(),
                Executors = _userService.GetUsers(2).OrderBy(item => item.LastName).ToList()
            });
        }

        [Authorize(Roles = RoleNames.Admin)]
        public IActionResult Admin()
        {
            return RedirectToAction("Index", "Admin");
        }

        private SessionContext? RequireSession()
        {
            return User.ToSessionContext();
        }
    }
}
