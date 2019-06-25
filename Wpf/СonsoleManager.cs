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
                { "specialCommand", "" },
                { "valueSpecialCommand", "" },
                { "secondSpecialCommand", "" },
                { "secondValueSpecialCommand", "" },
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
                case "disconnect":
                    Disconnect();
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
                    MainWindow mainWindow = new MainWindow();
                    
                    await ApiManager.Create("api/chat/create", $"{{'Name':'{Command["objectName"]}', 'Creator':'{User.Name}'}}");
                    await ApiManager.Create("api/chat/user", $"{{ 'Chat':{{'Name':'{Command["objectName"]}'}}, 'User':{{'Name':'{User.Name}'}} }}");

                    chatWindow.AddTabItem(Command["objectName"]);

                    mainWindow.UpdateUsersBox();
                    mainWindow.AddUserToChat(Command["objectName"]);

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
                    
                    ApiManager.Delete("api/chat", $"deleteChat/{Command["objectName"]}/{User.Name}");

                    chatWindow.DeleteTabItem(Command["objectName"]);

                    break;
                default:
                    MessageBox.Show("Неизвестный объект");
                    break;
            }
        }

        private async void Connect()
        {
            switch (Command["object"])
            {
                case "room":
                    bool isBanned = Convert.ToBoolean(await ApiManager.Read($"api/chat/{Command["objectName"]}/{User.Name}"));
                    if (!isBanned)
                    {
                        ChatControl chatWindow = new ChatControl(ChatControl);
                        MainWindow mainWindow = new MainWindow();

                        await ApiManager.Create("api/chat/user", $"{{ 'Chat':{{'Name':'{Command["objectName"]}'}}, 'User':{{'Name':'{User.Name}'}} }}");

                        chatWindow.AddTabItem(Command["objectName"]);

                        mainWindow.UpdateUsersBox();
                        mainWindow.AddUserToChat(Command["objectName"]);
                    }
                    else
                        MessageBox.Show("Вы на время забанены в этом чате.");

                    break;
                default:
                    MessageBox.Show("Неизвестный объект.");
                    break;
            }
        }
        
        private async void Disconnect()
        {
            if (Command["object"] == "room")
            {
                if ()
                string chatName = "";

                if (Command["objectName"] == string.Empty)
                    chatName = ChatSelected;
                else
                    chatName = Command["objectName"];

                if (chatName != string.Empty)
                {
                    ChatControl chatWindow = new ChatControl(ChatControl);
                    MainWindow mainWindow = new MainWindow();
                    string userName = User.Name;
                    string disabledUser = "";
                    double time = 0;
                    
                    if (Command["specialCommand"] != string.Empty && Command["secondSpecialCommand"] != string.Empty)
                    {
                        if (Command["objectName"] == string.Empty)
                            chatName = ChatSelected;
                        else
                            chatName = Command["objectName"];

                        //disabledUser = Command["valueSpecialCommand"];
                        time = Convert.ToDouble(Command["secondValueSpecialCommand"]);
                        userName = Command["valueSpecialCommand"];

                        await ApiManager.Delete("api/chat", $"removeUserFromChat/{chatName}/{User.Name}/{userName}/{time}");

                        //chatWindow.DeleteTabItem(chatName);

                        mainWindow.BanUserToChat(chatName, userName);
                    }
                    else
                    {


                        await ApiManager.Delete("api/chat", $"removeUserFromChat/{chatName}/{User.Name}");

                        chatWindow.DeleteTabItem(chatName);

                        mainWindow.RemoveUserFromChat(chatName, userName);
                    }
                }
                else
                    MessageBox.Show("Вы ввели несуществующее имя или не подключились к чату.");
            }
            else
                MessageBox.Show("Неизвестный объект");
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
