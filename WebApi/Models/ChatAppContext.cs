using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class ChatAppContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Chats> Chats { get; set; }
        public ChatAppContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-BNION4N\SQLEXPRESS;Database=ChatApp;Trusted_Connection=True;");
        }
        
        public void LoginOrLogout(User user, string status)
        {
            using (ChatAppContext db = new ChatAppContext())
            {
                User changedUser = db.Users.FirstOrDefault(u => u.Name == user.Name && u.Password == user.Password);

                changedUser.Status = status;

                db.Users.Update(changedUser);
                db.SaveChanges();
                
            }
        }

        public void Register(User user, string status)
        {
            using (ChatAppContext db = new ChatAppContext())
            {
                User newUser = new User { Name = user.Name, Password = user.Password, Status = status };

                db.Users.Add(newUser);
                db.SaveChanges();
            }
        }

        public bool IsRegistred(string name, string password)
        {
            using (ChatAppContext db = new ChatAppContext())
            {
                User checkedUser = db.Users.FirstOrDefault(u => u.Name == name && u.Password == password);

                if (checkedUser == null)
                    return false;

                return true;
            }
        }

        public void Create(Chats chat)
        {
            using (ChatAppContext db = new ChatAppContext())
            {
                Chats newChat = new Chats { Name = chat.Name, Creator = chat.Creator};

                db.Chats.Add(newChat);
                db.SaveChanges();
            }
        }

        /*
         *  Для Postman'а
         */
        public List<User> GetUsers()
        {
            using (ChatAppContext db = new ChatAppContext())
            {
                List<User> allUsers = db.Users.ToList();
                
                return allUsers;
            }
        }

        public List<Chats> GetChats()
        {
            using (ChatAppContext db = new ChatAppContext())
            {
                List<Chats> allChats = db.Chats.ToList();

                return allChats;
            }
        }
    }
}
