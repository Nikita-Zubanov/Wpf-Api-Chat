using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class User
    {
        public int Id { get; set; }         //использовать GUID
        public string Name { get; set; }

        private static int id;
        public void AutoIncrement()
        {
            Id = ++id;
        }
    }
}
