using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
            WriteConsoleLineToCommand();

            switch (Command["object"])
            {
                case "room":
                    RoomAction();

                    break;
                default:
                    MessageBox.Show("Неизвестный объект команды.");
                    break;
            }
        }

        private void RoomAction()
        {
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
                    if (Command["specialCommand"] == string.Empty)
                        Disconnect();
                    else
                        Ban();

                    break;
                default:
                    MessageBox.Show("Некорректная команда.");
                    break;
            }
        }
        private async void Create()
        {
            if (Command["objectName"] != string.Empty)
            {
                ChatControl chatWindow = new ChatControl(ChatControl);
                MainWindow mainWindow = new MainWindow();
                string chatName = Command["objectName"];
                string userName = User.Name;

                await ApiManager.Create("api/chat/create", $"{{'Name':'{chatName}', 'Creator':'{userName}'}}");
                await ApiManager.Create("api/chat/user", $"{{ 'Chat':{{'Name':'{chatName}'}}, 'User':{{'Name':'{userName}'}} }}");

                chatWindow.AddTabItem(chatName);
                mainWindow.UpdateUsersBox();

                mainWindow.AddUserToChat(chatName, userName);
            }
            else
                MessageBox.Show("Некорректное имя.");
        }
        private void Delete()
        {
            ChatControl chatWindow = new ChatControl(ChatControl);
            string chatName = Command["objectName"];
            string userName = User.Name;

            if (chatName != string.Empty)
            {
                ApiManager.Delete("api/chat", $"deleteChat/{chatName}/{userName}");

                chatWindow.DeleteTabItem(chatName);
            }
            else
                MessageBox.Show("Некорректное имя.");
        }
        private async void Connect()
        {
            if (Command["objectName"] != string.Empty)
            {
                string chatName = Command["objectName"];
                string userName = User.Name;
                bool isBanned = Convert.ToBoolean(await ApiManager.Read($"api/chat/isUserBanned/{chatName}/{userName}"));

                if (!isBanned)
                {
                    ChatControl chatWindow = new ChatControl(ChatControl);
                    MainWindow mainWindow = new MainWindow();

                    await ApiManager.Create("api/chat/user", $"{{ 'Chat':{{'Name':'{chatName}'}}, 'User':{{'Name':'{userName}'}} }}");

                    chatWindow.AddTabItem(chatName);
                    mainWindow.UpdateUsersBox();

                    mainWindow.AddUserToChat(chatName, userName);
                }
                else
                    MessageBox.Show("Вы на время забанены в этом чате.");
            }
            else
                MessageBox.Show("Некорректное имя.");
        }
        private async void Disconnect()
        {
            ChatControl chatWindow = new ChatControl(ChatControl);
            MainWindow mainWindow = new MainWindow();
            string chatName = Command["objectName"];
            string userName = User.Name;

            if (chatName == string.Empty)
                chatName = ChatSelected;

            await ApiManager.Delete("api/chat", $"removeUserFromChat/{chatName}/{userName}");

            chatWindow.DeleteTabItem(chatName);

            mainWindow.RemoveUserFromChat(chatName, userName);
        }
        private async void Ban()
        {
            if (Command["specialCommand"] == "-l" &&
                Command["valueSpecialCommand"] != string.Empty &&
                Command["secondSpecialCommand"] == "-m" &&
                Command["secondValueSpecialCommand"] != string.Empty &&
                Command["objectName"] != string.Empty)
            {
                string chatName = Command["objectName"];
                string userName = User.Name;
                bool hasRight = Convert.ToBoolean(await ApiManager.Read($"api/chat/isUserHasRights/{chatName}/{userName}"));

                if (hasRight)
                {
                    MainWindow mainWindow = new MainWindow();
                    double time = Convert.ToDouble(Command["secondValueSpecialCommand"]);
                    string userBannedName = Command["valueSpecialCommand"];
                    
                    await ApiManager.Change($"api/chat/banUserToChat/{time}", $"{{ 'Chat':{{'Name':'{chatName}'}}, 'User':{{'Name':'{userBannedName}'}} }}");

                    mainWindow.BanUserToChat(chatName, userBannedName);
                }
                else
                    MessageBox.Show("У вас нет прав на это действие.");
            }
            else
                MessageBox.Show("Некорректное(-ая) имя/команда или отсутствует подключение к чату.");
        }

        private void WriteConsoleLineToCommand()
        {
            string command = ConsoleLine;
            string[] words = Regex.Split(command, " ");

            for (int i = 0; i < words.Length; i++)
                Command[Command.ElementAt(i).Key] = words[i].ToLower();
        }
    }
}