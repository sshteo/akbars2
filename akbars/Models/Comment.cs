using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace akbars.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public int TicketId { get; set; }

        public int AuthorId { get; set; }

        public string CommentText { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}