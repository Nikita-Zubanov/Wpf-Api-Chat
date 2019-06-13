using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class UserChat
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int ChatsId { get; set; }
        public Chats Chats { get; set; }
    }
}
