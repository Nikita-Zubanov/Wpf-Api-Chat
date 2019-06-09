using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class ChatsContext
    {
        private static ChatsContext Chatsstack;       // Не нужно
        private List<Chats> Chats;

        private ChatsContext()
        {
            Chats = new List<Chats>();
        }

        public static ChatsContext GetChatsStack()    // Не нужно
        {
            if (Chatsstack == null)
                Chatsstack = new ChatsContext();

            return Chatsstack;
        }

        public void Post(Chats obj)
        {
            Chats.Add(obj);
        }
    }
}
