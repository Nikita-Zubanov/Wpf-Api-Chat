using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class BannedUserChat
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime BanEndDate { get; set; }

        public int ChatId { get; set; }
        public Chat Chat { get; set; }
    }
}
