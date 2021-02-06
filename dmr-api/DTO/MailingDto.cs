using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMR_API.DTO
{
    public class MailingDto
    {
        public int ID { get; set; }
        public List<string> UserNames { get; set; }
        public List<int> UserIDList { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public int UserID { get; set; }
        public string Frequency { get; set; }
        public DateTime TimeSend { get; set; }
    }
}
