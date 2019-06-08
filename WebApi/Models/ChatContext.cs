using System;
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
