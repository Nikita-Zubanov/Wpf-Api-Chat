using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Wpf
{
    class СonsoleManager : MainWindow
    {
        private readonly string ConsoleLine;
        private readonly Dictionary<string, string> Command;
        private readonly TabControl ChatControl;

        public СonsoleManager(TabControl chatControl, string concoleLine)
        {
            ChatControl = chatControl;
            ConsoleLine = concoleLine;
            Command = new Dictionary<string, string>
            {
                { "object", "" },
                { "action", "" },
                { "objectName", "" },
                { "specialCommand", "" }
            };
            
        }

        public void ExecuteCommand()
        {
            SetCommand();

            switch (Command["action"])
            {
                case "create":
                    Create();
                    break;
                case "remove":
                    Delete();
                    break;
                case "connect":
                    Connect();
                    break;
                default:
                    MessageBox.Show("Неизвестная команда");
                    break;
            }
        }

        private async void Create()
        {
            switch (Command["object"]) {
                case "room":
                    ChatControl chatWindow = new ChatControl(ChatControl);
                    chatWindow.AddTabItem(Command["objectName"]);

                    await ApiManager.Create("api/chat/create", $"{{'Name':'{Command["objectName"]}', 'Creator':'{User.Name}'}}");
                    ApiManager.Create("api/chat/user", $"{{ 'Chat':{{'Name':'{Command["objectName"]}'}}, 'User':{{'Name':'{User.Name}'}} }}");

                    break;
                default:
                    MessageBox.Show("Неизвестный объект");
                    break;
            }
        }
        private void Delete()
        {
            switch (Command["object"])
            {
                case "room":
                    ChatControl chatWindow = new ChatControl(ChatControl);
                    chatWindow.DeleteTabItem(Command["objectName"]);

                    ApiManager.Delete("api/chat", $"{Command["objectName"]}/{User.Name}");
                    break;
                default:
                    MessageBox.Show("Неизвестный объект");
                    break;
            }
        }
        private void Connect()
        {
            switch (Command["object"])
            {
                case "room":
                    ChatControl chatWindow = new ChatControl(ChatControl);
                    chatWindow.AddTabItem(Command["objectName"]);
                    
                    ApiManager.Create("api/chat/user", $"{{ 'Chat':{{'Name':'{Command["objectName"]}'}}, 'User':{{'Name':'{User.Name}'}} }}");

                    break;
                default:
                    MessageBox.Show("Неизвестный объект");
                    break;
            }
        }

        private void SetCommand()
        {
            string command = ConsoleLine;
            string[] words = Regex.Split(command, " ");

            for (int i = 0; i < words.Length; i++)
                Command[Command.ElementAt(i).Key] = words[i].ToLower();
        }
    }
}
