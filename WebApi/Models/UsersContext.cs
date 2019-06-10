using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class UsersContext
    {
        private static List<User> Users = new List<User>();
        
        public void LoginOrLogout(User user, string status)
        {
            var userByName = Users.Find(u => u.Name == user.Name);
            var index = Users.IndexOf(userByName);

            Users[index].Status = status;
        }

        public void Register(User user, string status)
        {
            user.AutoIncrement();
            Users.Add(user);

            Users.Last().Status = status;
        }

        public bool IsRegistred(string name, string password)
        {
            if (Users.Any(u => (u.Name == name && u.Password == password)))
                return true;

            return false;
        }

        /*
         *  Для Postman'а
         */
        public List<User> Get()
        {
            return Users;
        }
    }
}
