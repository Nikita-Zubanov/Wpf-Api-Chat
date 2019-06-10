using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class ChatsContext
    {
        private static List<Chats> Chats = new List<Chats>();

        public void Create(Chats obj)
        {
            obj.AutoIncrement();
            Chats.Add(obj);
        }

        /*
         *  Для Postman'а
         */
        public List<Chats> Get() 
        {
            return Chats;
        }
    }
}
