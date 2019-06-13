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
        public DbSet<Chat> Chat { get; set; }
        public DbSet<UsersInChats> UsersInChats { get; set; }

        public DbSet<UserChat> UsersChats { get; set; }

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

            //modelBuilder.Entity<User>().Ignore(b => b.UserChat);
            //modelBuilder.Entity<Chats>().Ignore(b => b.UserChat);
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
                User newUser = new User { Name = user.Name, Password = user.Password, Role = role, Status = status, UserChat = new List<UserChat>()};

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
            //using (ChatAppContext db = new ChatAppContext())
            //{
            //    User u1 = new User { Name = "Tom" };
            //    User u2 = new User { Name = "Alice" };
            //    db.Users.AddRange(new List<User> { u1, u2 });

            //    Chats c1 = new Chats { Name = "Алгоритмы" };
            //    Chats c2 = new Chats { Name = "Основы программирования" };
            //    db.Chats.AddRange(new List<Chats> { c1, c2 });

            //    db.SaveChanges();

            //    // добавляем к студентам курсы
            //    u1.UserChat.Add(new UserChat { ChatsId = c1.Id, UserId = u1.Id });
            //    u2.UserChat.Add(new UserChat { ChatsId = c2.Id, UserId = u2.Id });
            //    db.SaveChanges();
            //}
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
            //using (ChatAppContext db = new ChatAppContext()) 
            //{ 
            // UsersInChats newUserInChat = new UsersInChats { ChatName = userInChat.ChatName, UserName = userInChat.UserName }; 

            // db.UsersInChats.Add(newUserInChat); 
            // db.SaveChanges(); 
            //} 
            using (ChatAppContext db = new ChatAppContext())
            {
                User user = db.Users.FirstOrDefault(u => u.Name == userChat.UserName);
                Chats chat = db.Chats.FirstOrDefault(c => c.Name == userChat.ChatName);
                UserChat newUserChat = new UserChat { ChatsId = chat.Id, Chats = chat, UserId = user.Id, User = user };

                user.UserChat.Add(newUserChat);
                chat.UserChat.Add(newUserChat);

                db.Users.Update(user);
                db.Chats.Update(chat);

                db.UsersChats.Add(newUserChat);

                db.SaveChanges();

                //User user = db.Users.FirstOrDefault(u => u.Name == userChat.UserName);
                //Chats chat = db.Chats.FirstOrDefault(c => c.Name == userChat.ChatName);
                //UserChat newUserChat = new UserChat { ChatsId = chat.Id, Chats = chat, UserId = user.Id, User = user };

                //user.UserChat.Add(newUserChat);
                //chat.UserChat.Add(newUserChat);

                //db.UsersChats.Add(newUserChat);

                //db.SaveChanges();
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
                List<Chats> allChats = db.Chats.ToList();

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
