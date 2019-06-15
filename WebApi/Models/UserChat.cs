using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class UserChat
    {
        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int ChatId { get; set; }
        public virtual Chat Chat { get; set; }
    }
}
