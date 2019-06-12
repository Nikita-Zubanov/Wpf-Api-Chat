using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class UsersContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public UsersContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-BNION4N\SQLEXPRESS;Database=ChatApp;Trusted_Connection=True;");
        }

        //private static List<User> Users = new List<User>();
        
        public void LoginOrLogout(User user, string status)
        {
            //var userByName = Users.Find(u => u.Name == user.Name);
            //var index = Users.IndexOf(userByName);

            //Users[index].Status = status;
            using (UsersContext db = new UsersContext())
            {
                // получаем первый объект
                User changedUser = db.Users.FirstOrDefault(u => u.Name == user.Name && u.Password == user.Password);

                changedUser.Status = status;

                db.Users.Update(changedUser);
                db.SaveChanges();
                
            }
        }

        public void Register(User user, string status)
        {
            //user.AutoIncrement();
            //Users.Add(user);

            //Users.Last().Status = status;
            using (UsersContext db = new UsersContext())
            {
                User newUser = new User { Name = user.Name, Password = user.Password, Status = status };
                // Добавление
                db.Users.Add(newUser);
                db.SaveChanges();
            }
        }

        public bool IsRegistred(string name, string password)
        {
            //if (Users.Any(u => (u.Name == name && u.Password == password)))
            //    return true;

            //return false;
            using (UsersContext db = new UsersContext())
            {
                // получаем объекты из бд и выводим на консоль
                User checkedUser = db.Users.FirstOrDefault(u => u.Name == name && u.Password == password);

                if (checkedUser == null)
                    return false;

                return true;
            }
        }

        /*
         *  Для Postman'а
         */
        public List<User> Get()
        {
            using (UsersContext db = new UsersContext())
            {
                // получаем объекты из бд и выводим на консоль
                List<User> allUsers = db.Users.ToList();
                
                return allUsers;
            }
        }
    }
}
