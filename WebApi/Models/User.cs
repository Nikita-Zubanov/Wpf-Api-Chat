using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class User
    {
        public int Id { get; set; }         //использовать GUID
        public string Name { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }

        //public List<UserChat> UserChat { get; set; }

        //public User()
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
