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

        public void AddMessage(Chat chat)
        {
            //if (IsUserInChat(chat)) 
            // Chat.Add(chat); 
            if (IsUserInChat(chat.Name, chat.Author))
                using (ChatAppContext db = new ChatAppContext())
                {
                    Chat newMessage = new Chat { Name = chat.Name, Author = chat.Author, Message = chat.Message };

                    db.Chat.Add(newMessage);
                    db.SaveChanges();
                }
        }
        public void AddUser(UsersInChats userInChat)
        {
            //UsersInChats.Add(userInChat); 
            using (ChatAppContext db = new ChatAppContext())
            {
                UsersInChats newUserInChat = new UsersInChats { ChatName = userInChat.ChatName, UserName = userInChat.UserName };

                db.UsersInChats.Add(newUserInChat);
                db.SaveChanges();
            }
        }
        public List<Chat> GetChat(string name)
        {
            //List<Chat> currentChat = new List<Chat>(); 

            //for (int i = 0; i < Chat.Count; i++) 
            // if (Chat[i].Name == name) 
            // currentChat.Add(Chat[i]); 

            //return currentChat; 

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
            //List<UsersInChats> users = UsersInChats.FindAll(u => u.ChatName == name); 

            //return users; 

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
            //UsersInChats userInChat = UsersInChats.Find(uic => (uic.ChatName == chat.Name) && (uic.UserName == chat.Author)); 
            //if (userInChat == null) 
            // return false; 

            //return true; 
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
