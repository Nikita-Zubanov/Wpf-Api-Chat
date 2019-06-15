using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string ChatName { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }

        public int ChatId { get; set; }
        public Chat Chat { get; set; }
    }
}
