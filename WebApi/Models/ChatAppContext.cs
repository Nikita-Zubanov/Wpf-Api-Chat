﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class ChatAppContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        
        private const string serverName = @"DESKTOP-BNION4N\SQLEXPRESS";

        public ChatAppContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer($"Server={serverName};Database=ChatApp;Trusted_Connection=True;");
        }

        #region Model-config
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User[]
                {
                new User { Id = 1, Name = "admin", Password = "admin", Role = "administrator", Status = "Online" }
                });
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserChat>()
                .HasKey(uc => new { uc.ChatId, uc.UserId });
            modelBuilder.Entity<UserChat>()
                .HasOne(uc => uc.User)
                .WithMany(c => c.UserChats)
                .HasForeignKey(uc => uc.ChatId);
            modelBuilder.Entity<UserChat>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserChats)
                .HasForeignKey(uc => uc.UserId);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId);
        }
        #endregion

        #region Authorization methods
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
        #endregion
        
        #region Chat CRUD-methods
        public void Create(Chat chat)
        {
            using (ChatAppContext db = new ChatAppContext())
            {
                Chat newChat = new Chat { Name = chat.Name, Creator = chat.Creator };

                db.Chats.Add(newChat);
                db.SaveChanges();
            }
        }

        public void Delete(string chatName, string userName)
        {
            using (ChatAppContext db = new ChatAppContext())
            {
                User user = db.Users.Include(s => s.UserChats).FirstOrDefault(s => s.Name == userName);
                Chat chat = db.Chats.Include(s => s.UserChats).FirstOrDefault(s => s.Name == chatName);
                if (chat.Creator == userName)
                {
                    UserChat newUserChat = new UserChat { ChatId = chat.Id, UserId = user.Id };

                    chat.UserChats.Remove(newUserChat);
                    user.UserChats.Remove(newUserChat);

                    db.Remove(chat);

                    db.SaveChanges();
                }
            }
        }
        #endregion

        #region UserChat and Message methods
        public void AddMessageToChat(Message message)
        {
            if (IsUserInChat(message.ChatName, message.Author))
                using (ChatAppContext db = new ChatAppContext())
                {
                    Chat chat = db.Chats.Include(s => s.Messages).FirstOrDefault(s => s.Name == message.ChatName);
                    Message newMessage = new Message { ChatName = message.ChatName, Author = message.Author, Text = message.Text };

                    chat.Messages.Add(newMessage);

                    db.SaveChanges();
                }
        }

        public void AddUserToChat(UserChat userChat)
        {
            using (ChatAppContext db = new ChatAppContext())
            {
                Chat chat = db.Chats.Include(s => s.UserChats).FirstOrDefault(s => s.Name == userChat.Chat.Name);
                User user = db.Users.Include(s => s.UserChats).FirstOrDefault(s => s.Name == userChat.User.Name);
                UserChat newUserChat = new UserChat { ChatId = chat.Id, UserId = user.Id, BanEndDate = new DateTime() };

                if (user.UserChats.Select(uc => uc.ChatId).FirstOrDefault() != newUserChat.ChatId)
                {
                    chat.UserChats.Add(newUserChat);
                    user.UserChats.Add(newUserChat);

                    db.SaveChanges();
                }
            }
        }

        public void BanUserToChat(UserChat userChatBanned, double time)
        {
            using (ChatAppContext db = new ChatAppContext())
            {
                User userBanned = db.Users.Include(s => s.UserChats).FirstOrDefault(s => s.Name == userChatBanned.User.Name);
                Chat chat = db.Chats.Include(s => s.UserChats).FirstOrDefault(s => s.Name == userChatBanned.Chat.Name);
                UserChat userChat = userBanned.UserChats.Where(uc => uc.ChatId == chat.Id && uc.UserId == userBanned.Id).FirstOrDefault();

                userChat.BanEndDate = DateTime.Now.AddMinutes(time);

                db.SaveChanges();
            }
        }

        public void RemoveUserFromChat(string chatName, string userName)
        {
            using (ChatAppContext db = new ChatAppContext())
            {
                User user = db.Users.Include(s => s.UserChats).FirstOrDefault(s => s.Name == userName);
                Chat chat = db.Chats.Include(s => s.UserChats).FirstOrDefault(s => s.Name == chatName);
                UserChat userChat = user.UserChats.Where(uc => uc.ChatId == chat.Id && uc.UserId == user.Id).FirstOrDefault();

                chat.UserChats.Remove(userChat);
                user.UserChats.Remove(userChat);

                db.SaveChanges();
            }
        }

        public List<Message> GetMessages(string chatName)
        {
            using (ChatAppContext db = new ChatAppContext())
            {
                List<Message> currentChat = new List<Message>();
                List<Message> messages = db.Messages.Where(m => m.ChatName == chatName).ToList();

                return messages;
            }
        }

        public List<User> GetUsers(string chatName)
        {
            using (ChatAppContext db = new ChatAppContext())
            {
                List<User> users = new List<User>();
                Chat chat = db.Chats.Include(u => u.UserChats).FirstOrDefault(c => c.Name == chatName);
                List<UserChat> userChats = chat.UserChats.Where(c => c.Chat.Name == chatName).ToList();

                if (userChats.Count != 0)
                    foreach (UserChat userChat in userChats)
                    {
                        User user = db.Users.Include(s => s.UserChats).FirstOrDefault(s => s.Id == userChat.UserId);
                        users.Add(new User { Name = user.Name, Role = user.Role, Status = user.Status });
                    }

                return users;
            }
        }
        #endregion

        #region User-methods
        public void RenameUser(User user, string newUserName)
        {
            using (ChatAppContext db = new ChatAppContext())
            {
                User userChanged = db.Users.Where(u => u.Name == user.Name).FirstOrDefault();
                List<Chat> chats = db.Chats.Where(c => c.Creator == user.Name).ToList();
                List<Message> messages = db.Messages.Where(m => m.Author == user.Name).ToList();

                userChanged.Name = newUserName;
                for (int i = 0; i < chats.Count; i++)
                    chats[i].Creator = newUserName;
                for (int i = 0; i < messages.Count; i++)
                    messages[i].Author = newUserName;

                db.SaveChanges();
            }
        }
        #endregion

        #region Boolean-methods
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

        private bool IsUserInChat(string chatName, string userName)
        {
            List<User> users = GetUsers(chatName);

            foreach (User user in users)
                if (user.Name == userName)
                    return true;

            return false;
        }

        public bool IsUserBanned(string chatName, string userName)
        {
            using (ChatAppContext db = new ChatAppContext())
            {
                Chat chat = db.Chats.Include(u => u.UserChats).FirstOrDefault(c => c.Name == chatName);
                User user = db.Users.Include(u => u.UserChats).FirstOrDefault(c => c.Name == userName);
                UserChat userChat = chat.UserChats.Where(uc => uc.ChatId == chat.Id && uc.UserId == user.Id).FirstOrDefault();

                if (userChat != null && userChat.BanEndDate > DateTime.Now)
                    return true;

                return false;
            }
        }

        public bool HasRightToChat(string chatName, string userName)
        {
            using (ChatAppContext db = new ChatAppContext())
            {
                User user = db.Users.Where(u => u.Name == userName).FirstOrDefault();
                Chat chat = db.Chats.Where(u => u.Name == chatName).FirstOrDefault();

                if (chat.Creator == user.Name || user.Role == "administrator" || user.Name == "moderator")
                    return true;

                return false;
            }
        }

        public bool IsUserHasRight(string userName, string userPassword)
        {
            using (ChatAppContext db = new ChatAppContext())
            {
                User userChanged = db.Users.Where(u => u.Name == userName).FirstOrDefault();
                User user = db.Users.Where(u => u.Password == userPassword).FirstOrDefault();

                if (user.Role == "administrator" || userChanged.Password == userPassword)
                    return true;

                return false;
            }
        }

        public bool IsUserExists(string userName)
        {
            using (ChatAppContext db = new ChatAppContext())
            {
                User user = db.Users.Where(u => u.Name == userName).FirstOrDefault();

                if (user != null)
                    return true;

                return false;
            }
        }

        public bool IsChatExists(string chatName)
        {
            using (ChatAppContext db = new ChatAppContext())
            {
                Chat chat = db.Chats.Where(u => u.Name == chatName).FirstOrDefault();

                if (chat != null)
                    return true;

                return false;
            }
        }
        #endregion
    }
}