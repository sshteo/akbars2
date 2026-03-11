using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace akbars.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string ShortDescription { get; set; }

        public string DetailedDescription { get; set; }

        public int PriorityId { get; set; }

        public int TypeId { get; set; }

        public int StatusId { get; set; }

        public int AuthorId { get; set; }

        public int AssigneeId { get; set; }
    }
}
