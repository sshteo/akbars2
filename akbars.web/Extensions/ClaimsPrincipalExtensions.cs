using System.Security.Claims;
using akbars.Models;

namespace akbars.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static SessionContext? ToSessionContext(this ClaimsPrincipal principal)
        {
            if (principal?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            var userIdValue = principal.FindFirstValue("user_id");
            var roleIdValue = principal.FindFirstValue("role_id");

            if (!int.TryParse(userIdValue, out var userId) || !int.TryParse(roleIdValue, out var roleId))
            {
                return null;
            }

            return new SessionContext
            {
                UserId = userId,
                FullName = principal.FindFirstValue("full_name") ?? string.Empty,
                FirstName = principal.FindFirstValue("first_name") ?? string.Empty,
                LastName = principal.FindFirstValue("last_name") ?? string.Empty,
                MiddleName = principal.FindFirstValue("middle_name") ?? string.Empty,
                Email = principal.FindFirstValue("email") ?? string.Empty,
                Phone = principal.FindFirstValue("phone") ?? string.Empty,
                Department = principal.FindFirstValue("department") ?? string.Empty,
                RoleId = roleId,
                RoleName = principal.FindFirstValue("role_name") ?? string.Empty
            };
        }
    }
}
