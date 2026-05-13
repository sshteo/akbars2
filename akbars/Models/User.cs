using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace akbars.Models
{
    public class User
    {
        public int Id { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Login { get; set; }

        public string PasswordHash { get; set; }

        public int RoleId { get; set; }

        public string Department { get; set; }

        public string RoleName { get; set; }

        public string FullName
        {
            get
            {
                var parts = new[] { LastName, FirstName, MiddleName }
                    .Where(part => !string.IsNullOrWhiteSpace(part));

                return string.Join(" ", parts);
            }
        }
    }
}
