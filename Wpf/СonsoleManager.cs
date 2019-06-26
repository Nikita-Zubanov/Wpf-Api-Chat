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
                case "user":
                    UserAction();

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
                SignalRManager signalRManager = new SignalRManager();
                string chatName = Command["objectName"];
                string userName = User.Name;

                bool isChatExists = Convert.ToBoolean(await ApiManager.Read($"api/chat/isChatExists/{chatName}"));
                if (!isChatExists)
                {
                    await ApiManager.Create("api/chat/create", $"{{'Name':'{chatName}', 'Creator':'{userName}'}}");
                    await ApiManager.Create("api/chat/user", $"{{ 'Chat':{{'Name':'{chatName}'}}, 'User':{{'Name':'{userName}'}} }}");

                    chatWindow.AddTabItem(chatName);
                    mainWindow.UpdateUsersBox();

                    signalRManager.AddUserToChat(chatName, userName);
                }
                else
                    MessageBox.Show("Чат с таким названием уже существует.");
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

                bool isChatExists = Convert.ToBoolean(await ApiManager.Read($"api/chat/isChatExists/{chatName}"));
                if (isChatExists)
                {
                    bool isBanned = Convert.ToBoolean(await ApiManager.Read($"api/chat/isUserBanned/{chatName}/{userName}"));
                    if (!isBanned)
                    {
                        ChatControl chatWindow = new ChatControl(ChatControl);
                        MainWindow mainWindow = new MainWindow();
                        SignalRManager signalRManager = new SignalRManager();

                        await ApiManager.Create("api/chat/user", $"{{ 'Chat':{{'Name':'{chatName}'}}, 'User':{{'Name':'{userName}'}} }}");

                        chatWindow.AddTabItem(chatName);
                        mainWindow.UpdateUsersBox();

                        signalRManager.AddUserToChat(chatName, userName);
                    }
                    else
                        MessageBox.Show("Вы на время забанены в этом чате.");
                }
                else
                    MessageBox.Show("Чата с таким названием не существует.");
            }
            else
                MessageBox.Show("Некорректное имя.");
        }
        private async void Disconnect()
        {
            ChatControl chatWindow = new ChatControl(ChatControl);
            MainWindow mainWindow = new MainWindow();
            SignalRManager signalRManager = new SignalRManager();
            string chatName = Command["objectName"];
            string userName = User.Name;

            if (chatName == string.Empty)
                chatName = ChatSelected;

            await ApiManager.Delete("api/chat", $"removeUserFromChat/{chatName}/{userName}");

            chatWindow.DeleteTabItem(chatName);

            signalRManager.RemoveUserFromChat(chatName, userName);
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

                bool hasRight = Convert.ToBoolean(await ApiManager.Read($"api/chat/hasRightToChat/{chatName}/{userName}"));
                if (hasRight)
                {
                    MainWindow mainWindow = new MainWindow();
                    SignalRManager signalRManager = new SignalRManager();
                    double time = Convert.ToDouble(Command["secondValueSpecialCommand"]);
                    string userBannedName = Command["valueSpecialCommand"];
                    
                    await ApiManager.Change($"api/chat/banUserToChat/{time}", $"{{ 'Chat':{{'Name':'{chatName}'}}, 'User':{{'Name':'{userBannedName}'}} }}");

                    signalRManager.BanUserToChat(chatName, userBannedName);
                }
                else
                    MessageBox.Show("У вас нет прав на это действие.");
            }
            else
                MessageBox.Show("Некорректное(-ая) имя/команда или отсутствует подключение к чату.");
        }

        private void UserAction()
        {
            switch (Command["action"])
            {
                case "rename":
                    Rename();

                    break;
                case "ban":
                    Delete();

                    break;
                case "moderator":
                    Connect();

                    break;
                default:
                    MessageBox.Show("Некорректная команда.");
                    break;
            }
        }
        private async void Rename()
        {
            if (Command["objectName"] != string.Empty)
            {
                MainWindow mainWindow = new MainWindow();
                SignalRManager signalRManager = new SignalRManager();
                string userName;
                string newUserName;

                if (Command["specialCommand"] == "-l" &&
                Command["valueSpecialCommand"] != string.Empty)
                {
                    userName = Command["objectName"];
                    newUserName = Command["valueSpecialCommand"];
                }
                else
                {
                    userName = User.Name;
                    newUserName = Command["objectName"];
                }

                bool isNewUserExists = Convert.ToBoolean(await ApiManager.Read($"api/chat/isUserExists/{newUserName}"));
                bool isUserExists = Convert.ToBoolean(await ApiManager.Read($"api/chat/isUserExists/{userName}"));
                if (!isNewUserExists && isUserExists)
                {
                    bool hasRight = Convert.ToBoolean(await ApiManager.Read($"api/chat/isUserHasRight/{userName}/{User.Password}"));
                    if (hasRight)
                    {
                        await ApiManager.Change($"api/chat/renameUser/{newUserName}", $"{{ 'Name':'{userName}' }}");

                        signalRManager.UpdateUser(userName, newUserName);
                    }
                    else
                        MessageBox.Show("У вас нет прав на это действие.");
                }
                else
                    MessageBox.Show("Вы пытаетесь переименовать несуществующего пользователя или пользователь с предложенным логином уже существует.");
            }
            else
                MessageBox.Show("Некорректное имя.");
        }

        private void WriteConsoleLineToCommand()
        {
            try
            {
                string command = ConsoleLine;
                string[] words = Regex.Split(command, " ");

                for (int i = 0; i < words.Length; i++)
                    Command[Command.ElementAt(i).Key] = words[i].ToLower();
            }
            catch
            {
                MessageBox.Show("Проверьте набранную команду.");
            }
        }
    }
}