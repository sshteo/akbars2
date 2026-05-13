namespace akbars.Extensions
{
    public static class RoleNames
    {
        public const string Employee = "Employee";
        public const string Executor = "Executor";
        public const string Dispatcher = "Dispatcher";
        public const string Admin = "Admin";

        public static string FromRoleId(int roleId)
        {
            return roleId switch
            {
                1 => Employee,
                2 => Executor,
                3 => Dispatcher,
                4 => Admin,
                _ => string.Empty
            };
        }
    }
}
