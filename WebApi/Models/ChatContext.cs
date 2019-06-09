﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class ChatContext : IDatastackContext<Chat>
    {
        private static ChatContext Chatstack;       // Не нужно
        private List<Chat> Chat;

        private ChatContext()
        {
            Chat = new List<Chat>();
        }

        public static ChatContext GetChatStack()    // Не нужно
        {
            if (Chatstack == null)
                Chatstack = new ChatContext();

            return Chatstack;
        }

        public void AddMessage(Chat obj)
        {
            Chat.Add(obj);
        }

        public List<Chat> GetCurrentChat(string name)
        {
            List<Chat> currentChat = new List<Chat>();
            for(int i = 0; i< Chat.Count; i++)
                if (Chat[i].Name == name)
                    currentChat.Add(Chat[i]);

            return currentChat;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public List<Chat> Get()
        {
            return Chat;
        }

        public Chat Get(string author)
        {
            var message = Chat.Find(u => u.Author == author);

            return message;
        }

        public void Post(Chat obj)
        {
            Chat.Add(obj);
        }

        public void Put(string author, Chat obj)
        {
            var message = Chat.Find(u => u.Author == author);
            var index = Chat.IndexOf(message);

            Chat[index] = obj;
        }

        public void Delete(string author)
        {
            var message = Chat.Find(u => u.Author == author);

            Chat.Remove(message);
        }


        public Chat Get(int id) => null;
        public void Put(int id, Chat obj) { }
        public void Delete(int id) { }
    }
}
