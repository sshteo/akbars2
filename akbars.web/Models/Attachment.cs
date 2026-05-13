using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace akbars.Models
{
    public class Attachment
    {
        public int Id { get; set; }

        public int TicketId { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public DateTime UploadedAt { get; set; }

        public int UploadedBy { get; set; }
    }
}