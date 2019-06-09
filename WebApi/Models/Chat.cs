using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class Chat
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Message { get; set; }
        public string UserNames { get; set; }
    }
}
