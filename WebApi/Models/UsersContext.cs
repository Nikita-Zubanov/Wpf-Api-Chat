using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class UsersContext : IDatastackContext<User>
    {
        private static UsersContext Userstack;
        private List<User> Users;

        private UsersContext()
        {
            Users = new List<User>();
        }

        public static UsersContext GetUsersStack()
        {
            if (Userstack == null)
                Userstack = new UsersContext();

            return Userstack;
        }

        public List<User> Get()
        {
            return Users;
        }

        public User Get(int id)
        {
            var user = Users.Find(u => u.Id == id);

            return user;
        }

        public void Post(User obj)
        {
            obj.AutoIncrement();
            Users.Add(obj);
        }

        public void Put(int id, User obj)
        {
            var user = Users.Find(u => u.Id == id);
            var index = Users.IndexOf(user);

            Users[index] = obj;
        }
        public void Put(string name, User obj)
        {
            var user = Users.Find(u => u.Name == name);
            var index = Users.IndexOf(user);

            Users[index] = obj;
        }

        public void Delete(string value)
        {
            var user = Users.Find(u => u.Name == value);

            Users.Remove(user);
        }

        public User Get(string value) => null;
        
        public void Delete(int id) { }
    }
}
