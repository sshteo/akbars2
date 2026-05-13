namespace akbars.Models
{
    public class SessionContext
    {
        public int UserId { get; set; }

        public string FullName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Department { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; }
    }
}
