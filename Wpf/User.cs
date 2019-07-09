using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Wpf
{
    static class User
    {
        public static string Name { get; set; }
        public static string Password { get; set; }
        
        public const string administrator = "administrator";
        public const string moderator = "moderator";
        public const string user = "user";
        public const string creator = "creator";
        public const string banned = "banned";
        
        public static readonly SolidColorBrush administratorColor = new SolidColorBrush(Colors.Gold);
        public static readonly SolidColorBrush moderatorColor = new SolidColorBrush(Colors.Orange);
        public static readonly SolidColorBrush userColor = new SolidColorBrush(Colors.Black);
        public static readonly SolidColorBrush creatorColor = new SolidColorBrush(Colors.Gray);
        public static readonly SolidColorBrush bannedColor = new SolidColorBrush(Colors.Red);
    }
}
