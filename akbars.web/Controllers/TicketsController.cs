using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using akbars.Extensions;
using akbars.Models;
using akbars.Services;
using akbars.ViewModels;

namespace akbars.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly ILookupService _lookupService;
        private readonly IUserService _userService;

        public TicketsController(
            ITicketService ticketService,
            ILookupService lookupService,
            IUserService userService)
        {
            _ticketService = ticketService;
            _lookupService = lookupService;
            _userService = userService;
        }

        [Authorize(Roles = RoleNames.Employee)]
        [HttpGet]
        public IActionResult My(string? statusName, string? priorityName, string? searchText)
        {
            var session = SessionOrRedirect();
            if (session == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View("List", BuildListViewModel(
                session,
                "Мои заявки",
                "Создание, отслеживание и удаление неназначенных заявок.",
                new TicketQuery
                {
                    AuthorId = session.UserId,
                    StatusName = NormalizeFilter(statusName),
                    PriorityName = NormalizeFilter(priorityName),
                    SearchText = searchText
                },
                allowCreate: true,
                allowDelete: true,
                allowAssign: false,
                allowStatusActions: false));
        }

        [Authorize(Roles = RoleNames.Dispatcher)]
        [HttpGet]
        public IActionResult Queue(string? statusName, string? priorityName, string? searchText)
        {
            var session = SessionOrRedirect();
            if (session == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View("List", BuildListViewModel(
                session,
                "Очередь заявок",
                "Поиск, triage и назначение исполнителей.",
                new TicketQuery
                {
                    StatusName = NormalizeFilter(statusName),
                    PriorityName = NormalizeFilter(priorityName),
                    SearchText = searchText
                },
                allowCreate: false,
                allowDelete: false,
                allowAssign: true,
                allowStatusActions: false));
        }

        [Authorize(Roles = RoleNames.Executor)]
        [HttpGet]
        public IActionResult Assigned(string? statusName, string? priorityName, string? searchText)
        {
            var session = SessionOrRedirect();
            if (session == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View("List", BuildListViewModel(
                session,
                "Назначенные заявки",
                "Исполнение, статусные переходы и контроль просрочки.",
                new TicketQuery
                {
                    AssigneeId = session.UserId,
                    StatusName = NormalizeFilter(statusName),
                    PriorityName = NormalizeFilter(priorityName),
                    SearchText = searchText
                },
                allowCreate: false,
                allowDelete: false,
                allowAssign: false,
                allowStatusActions: true));
        }

        [Authorize(Roles = RoleNames.Employee)]
        [HttpGet]
        public IActionResult Create()
        {
            return View(BuildCreateModel());
        }

        [Authorize(Roles = RoleNames.Employee)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateTicketViewModel model)
        {
            var session = SessionOrRedirect();
            if (session == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                model.Priorities = _lookupService.GetPriorities();
                model.TicketTypes = _lookupService.GetTicketTypes();
                return View(model);
            }

            try
            {
                _ticketService.CreateTicket(
                    session.UserId,
                    model.ShortDescription,
                    model.DetailedDescription,
                    model.PriorityId,
                    model.TypeId);

                TempData["Success"] = "Заявка создана.";
                return RedirectToAction(nameof(My));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                model.Priorities = _lookupService.GetPriorities();
                model.TicketTypes = _lookupService.GetTicketTypes();
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var session = SessionOrRedirect();
            if (session == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var ticket = _ticketService.GetTicketDetails(id);
            if (ticket == null || !HasAccess(session, ticket))
            {
                return NotFound();
            }

            return View(new TicketDetailsViewModel
            {
                Session = session,
                Ticket = ticket,
                AllowStatusActions = session.RoleId == 2 && ticket.AssigneeId == session.UserId
            });
        }

        [Authorize(Roles = RoleNames.Employee)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var session = SessionOrRedirect();
            if (session == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!_ticketService.DeleteTicket(id, session.UserId))
            {
                TempData["Error"] = "Удалить можно только неназначенную заявку, созданную вами.";
            }
            else
            {
                TempData["Success"] = "Заявка удалена.";
            }

            return RedirectToAction(nameof(My));
        }

        [Authorize(Roles = RoleNames.Dispatcher)]
        [HttpGet]
        public IActionResult Assign(int id)
        {
            var ticket = _ticketService.GetTicketDetails(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(new AssignTicketViewModel
            {
                TicketId = id,
                TicketSummary = ticket.ShortDescription,
                Executors = _userService.GetUsers(2).OrderBy(item => item.LastName).ToList()
            });
        }

        [Authorize(Roles = RoleNames.Dispatcher)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Assign(AssignTicketViewModel model)
        {
            var session = SessionOrRedirect();
            if (session == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                model.Executors = _userService.GetUsers(2).OrderBy(item => item.LastName).ToList();
                return View(model);
            }

            try
            {
                _ticketService.AssignTicket(model.TicketId, model.ExecutorId, session.UserId);
                TempData["Success"] = "Исполнитель назначен.";
                return RedirectToAction(nameof(Queue));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                model.Executors = _userService.GetUsers(2).OrderBy(item => item.LastName).ToList();
                return View(model);
            }
        }

        [Authorize(Roles = RoleNames.Executor)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeStatus(int id, string actionName, string actionNote)
        {
            var session = SessionOrRedirect();
            if (session == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var ticket = _ticketService.GetTicketDetails(id);
            if (ticket == null || ticket.AssigneeId != session.UserId)
            {
                return NotFound();
            }

            try
            {
                switch (actionName)
                {
                    case "start":
                        _ticketService.StartTicket(id, session.UserId, actionNote);
                        TempData["Success"] = "Статус обновлен: В работе.";
                        break;
                    case "complete":
                        _ticketService.CompleteTicket(id, session.UserId, actionNote);
                        TempData["Success"] = "Заявка завершена.";
                        break;
                    case "cancel":
                        _ticketService.CancelTicket(id, session.UserId, actionNote);
                        TempData["Success"] = "Заявка отменена.";
                        break;
                    default:
                        TempData["Error"] = "Неизвестное действие.";
                        break;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Не удалось обновить статус: " + ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [Authorize(Roles = RoleNames.Dispatcher)]
        [HttpGet]
        public IActionResult Employees()
        {
            ViewData["Title"] = "Сотрудники";
            ViewData["Subtitle"] = "Каталог авторов заявок с контактными данными.";
            return View("UsersDirectory", _userService.GetUsers(1).OrderBy(item => item.LastName).ToList());
        }

        [Authorize(Roles = RoleNames.Dispatcher)]
        [HttpGet]
        public IActionResult Executors()
        {
            ViewData["Title"] = "Исполнители";
            ViewData["Subtitle"] = "Каталог исполнителей для назначения.";
            return View("UsersDirectory", _userService.GetUsers(2).OrderBy(item => item.LastName).ToList());
        }

        private TicketListViewModel BuildListViewModel(
            SessionContext session,
            string title,
            string subtitle,
            TicketQuery query,
            bool allowCreate,
            bool allowDelete,
            bool allowAssign,
            bool allowStatusActions)
        {
            return new TicketListViewModel
            {
                Session = session,
                Title = title,
                Subtitle = subtitle,
                SearchText = query.SearchText ?? string.Empty,
                StatusName = query.StatusName ?? string.Empty,
                PriorityName = query.PriorityName ?? string.Empty,
                AllowCreate = allowCreate,
                AllowDelete = allowDelete,
                AllowAssign = allowAssign,
                AllowStatusActions = allowStatusActions,
                StatusOptions = _lookupService.GetStatuses().Select(item => item.Name).ToList(),
                PriorityOptions = _lookupService.GetPriorities().Select(item => item.Name).ToList(),
                Tickets = _ticketService.GetTickets(query)
            };
        }

        private CreateTicketViewModel BuildCreateModel()
        {
            return new CreateTicketViewModel
            {
                Priorities = _lookupService.GetPriorities(),
                TicketTypes = _lookupService.GetTicketTypes()
            };
        }

        private SessionContext? SessionOrRedirect()
        {
            return User.ToSessionContext();
        }

        private static string? NormalizeFilter(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static bool HasAccess(SessionContext session, TicketDetails ticket)
        {
            return session.RoleId switch
            {
                1 => ticket.AuthorId == session.UserId,
                2 => ticket.AssigneeId == session.UserId,
                3 => true,
                4 => true,
                _ => false
            };
        }
    }
}
