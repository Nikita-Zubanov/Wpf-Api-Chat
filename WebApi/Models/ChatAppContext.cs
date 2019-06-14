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
        public DbSet<Chats> allChat { get; set; }
        public DbSet<Chat> Chat { get; set; }
        public DbSet<UsersInChats> UsersInChats { get; set; }

        //public DbSet<UserChat> UserChats { get; set; }

        public ChatAppContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-BNION4N\SQLEXPRESS;Database=ChatApp;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User[]
                {
                new User { Id=1, Name="admin", Password="admin", Role="admin", Status="Online" }
                });
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserChat>()
               .HasKey(k => new { k.UserId, k.ChatsId });

            modelBuilder.Entity<UserChat>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserChat)
                .HasForeignKey(uc => uc.UserId);

            modelBuilder.Entity<UserChat>()
                .HasOne(uc => uc.Chats)
                .WithMany(c => c.UserChat)
                .HasForeignKey(uc => uc.ChatsId);

            modelBuilder.Entity<UserChat>().Ignore(b => b.ChatName);
            modelBuilder.Entity<UserChat>().Ignore(b => b.UserName);
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
        public void Register(User user, string role, string status)
        {
            using (ChatAppContext db = new ChatAppContext())
            {
                User newUser = new User { Name = user.Name, Password = user.Password, Role = role, Status = status };

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
                Chats newChat = new Chats { Name = chat.Name, Creator = chat.Creator };

                db.allChat.Add(newChat);
                db.SaveChanges();
            }
        }

        public void AddMessage(Chat chat)
        {
            if (IsUserInChat(chat.Name, chat.Author))
                using (ChatAppContext db = new ChatAppContext())
                {
                    Chat newMessage = new Chat { Name = chat.Name, Author = chat.Author, Message = chat.Message };

                    db.Chat.Add(newMessage);
                    db.SaveChanges();
                }
        }
        public void AddUser(UserChat userChat)
        {
            using (ChatAppContext db = new ChatAppContext())
            {
                User user = db.Users.FirstOrDefault(u => u.Name == userChat.UserName);
                Chats chat = db.allChat.FirstOrDefault(c => c.Name == userChat.ChatName);
                UserChat newUserChat = new UserChat { ChatsId = chat.Id, UserId = user.Id };

                user.UserChat.Add(newUserChat);

                db.SaveChanges();
            }
        }
        public List<Chat> GetChat(string name)
        {
            using (ChatAppContext db = new ChatAppContext())
            {
                List<Chat> currentChat = new List<Chat>();

                List<Chat> allChats = db.Chat.ToList();
                for (int i = 0; i < allChats.Count; i++)
                    if (allChats[i].Name == name)
                        currentChat.Add(allChats[i]);

                return currentChat;
            }
        }
        public List<UsersInChats> GetUsers(string name)
        {
            using (ChatAppContext db = new ChatAppContext())
            {
                List<UsersInChats> usersInChat = new List<UsersInChats>();

                List<UsersInChats> allUsersInChats = db.UsersInChats.ToList();
                for (int i = 0; i < allUsersInChats.Count; i++)
                    if (allUsersInChats[i].ChatName == name)
                        usersInChat.Add(allUsersInChats[i]);

                return usersInChat;
            }
        }
        private bool IsUserInChat(string chatName, string userName)
        {
            using (ChatAppContext db = new ChatAppContext())
            {
                UsersInChats userInChat = db.UsersInChats.FirstOrDefault(u => u.ChatName == chatName && u.UserName == userName);

                if (userInChat == null)
                    return false;

                return true;
            }
        }

        /* 
        * Для Postman'а 
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
                List<Chats> allChats = db.allChat.ToList();

                return allChats;
            }
        }

        public List<Chat> GetChat()
        {
            using (ChatAppContext db = new ChatAppContext())
            {
                List<Chat> allChat = db.Chat.ToList();

                return allChat;
            }
        }
    }
}
