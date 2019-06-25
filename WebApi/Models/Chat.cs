using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Creator { get; set; }

        public virtual ICollection<UserChat> UserChats { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        //public virtual ICollection<BannedUserChat> BannedUserChats { get; set; }
    }
}
