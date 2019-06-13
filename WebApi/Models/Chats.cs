using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class Chats
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Creator { get; set; }

        //public List<UserChat> UserChat { get; set; }

        //public Chats()
        //{
        //    UserChat = new List<UserChat>();
        //}

        private static List<UserChat> userChat = new List<UserChat>();
        public List<UserChat> UserChat
        {
            get
            {
                return userChat;
            }
            set
            {
                userChat.Concat(value);
            }
        }
    }
}
