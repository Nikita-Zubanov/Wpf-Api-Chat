using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class ChatContext
    {
        private static List<Chat> Chat = new List<Chat>();
        private static List<UsersInChats> UsersInChats = new List<UsersInChats>();

        public void AddMessage(Chat chat)
        {
            if (IsUserInChat(chat))
                Chat.Add(chat);
        }

        public void AddUser(UsersInChats userInChat)
        {
            UsersInChats.Add(userInChat);
        }

        public List<Chat> GetChat(string name)
        {
            List<Chat> currentChat = new List<Chat>();

            for(int i = 0; i< Chat.Count; i++)
                if (Chat[i].Name == name)
                    currentChat.Add(Chat[i]);

            return currentChat;
        }

        public List<UsersInChats> GetUsers(string name)
        {
            //List<UsersInChats> users = new List<UsersInChats>();

            //for (int i = 0; i < UsersInChats.Count; i++)
            //    if (UsersInChats[i].ChatName == name)
            //        users.Add(UsersInChats[i]);

            List<UsersInChats> users = UsersInChats.FindAll(u => u.ChatName == name);
            //var index = Users.IndexOf(userByName);

            return users;
        }

        private bool IsUserInChat(Chat chat)
        {
            UsersInChats userInChat = UsersInChats.Find(uic => (uic.ChatName == chat.Name) && (uic.UserName == chat.Author));
            if (userInChat == null)
                return false;

            return true;
        }

        /*
         *  Для Postman'а
         */
        public List<Chat> Get()
        {
            return Chat;
        }
    }
}
