using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class UsersInChats
    {
        public int Id { get; set; }
        public string ChatName { get; set; }
        public string UserName  { get; set; }
    }
}
