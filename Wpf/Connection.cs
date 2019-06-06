using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf
{
    class Connection
    {
        private static Connection Connect;
        public bool IsConnected { get; set; }
        public string UserName { get; set; }

        private Connection() { }

        public static Connection GetConnection()
        {
            if (Connect == null)
                Connect = new Connection();

            return Connect;
        }

        public void Connecting(string name)
        {
            IsConnected = true;
            UserName = name;
        }

        public void Disconnecting()
        {
            IsConnected = false;
            UserName = "";
        }
    }
}
