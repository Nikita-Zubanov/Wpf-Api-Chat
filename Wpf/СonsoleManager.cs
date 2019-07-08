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
        private readonly TabControl ChatsControl;

        public СonsoleManager(TabControl chatControl, string concoleLine)
        {
            ChatsControl = chatControl;
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
                case "ybot":
                    BotAction();

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
                case "rename":
                    RenameChat();

                    break;
                case "connect":
                    Connect();

                    break;
                case "disconnect":
                    if (Command["specialCommand"] == string.Empty)
                        Disconnect();
                    else
                        BanToChat();

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
                ChatControl chatWindow = new ChatControl(ChatsControl);
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
        private async void Delete()
        {
            ChatControl chatWindow = new ChatControl(ChatsControl);
            string chatName = Command["objectName"];
            bool hasRight = Convert.ToBoolean(await ApiManager.Read($"api/chat/hasHighRightInChat/{chatName}/{User.Name}"));
            if (hasRight)
            {
                if (chatName != string.Empty)
                {
                    SignalRManager signalRManager = new SignalRManager();

                    ApiManager.Delete("api/chat", $"deleteChat/{chatName}/{User.Name}");

                    chatWindow.DeleteTabItem(chatName);

                    signalRManager.RemoveChat(chatName);
                }
                else
                    MessageBox.Show("Некорректное имя.");
            }
            else
                MessageBox.Show("У вас нет прав на это действие.");
        }
        private async void RenameChat()
        {
            if (Command["objectName"] != string.Empty && ChatSelected != string.Empty)
            {
                SignalRManager signalRManager = new SignalRManager();
                string chatName = ChatSelected;
                string newChatName = Command["objectName"];

                bool isChatExists = Convert.ToBoolean(await ApiManager.Read($"api/chat/isChatExists/{chatName}"));
                bool isNewChatExists = Convert.ToBoolean(await ApiManager.Read($"api/chat/isChatExists/{newChatName}"));
                if (isChatExists && !isNewChatExists)
                {
                    bool hasRight = Convert.ToBoolean(await ApiManager.Read($"api/chat/hasHighRightInChat/{chatName}/{User.Name}"));
                    if (hasRight)
                    {
                        await ApiManager.Change($"api/chat/renameChat/{newChatName}", $"{{ 'Name':'{chatName}' }}");

                        signalRManager.RenameChat(chatName, newChatName);
                    }
                    else
                        MessageBox.Show("У вас нет прав на это действие.");
                }
                else
                    MessageBox.Show("У вас нет подключения к комнате или комната с предложенным названием уже существует.");
            }
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
                        ChatControl chatWindow = new ChatControl(ChatsControl);
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
            ChatControl chatWindow = new ChatControl(ChatsControl);
            MainWindow mainWindow = new MainWindow();
            SignalRManager signalRManager = new SignalRManager();
            string chatName = Command["objectName"];
            string userName = User.Name;

            if (chatName == string.Empty)
                chatName = ChatSelected;
            if (chatName != string.Empty)
            {
                await ApiManager.Delete("api/chat", $"removeUserFromChat/{chatName}/{userName}");

                chatWindow.DeleteTabItem(chatName);

                signalRManager.RemoveUserFromChat(chatName, userName);
            }
            else
                MessageBox.Show("Подключитесь к комнате или введите корректное название.");
        }
        private async void BanToChat()
        {
            if (Command["specialCommand"] == "-l" &&
                Command["valueSpecialCommand"] != string.Empty &&
                Command["secondSpecialCommand"] == "-m" &&
                Command["secondValueSpecialCommand"] != string.Empty &&
                Command["objectName"] != string.Empty)
            {
                string chatName = Command["objectName"];
                string userName = User.Name;

                bool hasRight = Convert.ToBoolean(await ApiManager.Read($"api/chat/hasLowRightInChat/{chatName}/{userName}"));
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
                    RenameUser();

                    break;
                case "ban":
                    BanUser();

                    break;
                case "moderator":
                    Moderator();

                    break;
                default:
                    MessageBox.Show("Некорректная команда.");
                    break;
            }
        }
        private async void RenameUser()
        {
            if (Command["objectName"] != string.Empty)
            {
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
                    bool hasRight = Convert.ToBoolean(await ApiManager.Read($"api/chat/isTrueUser/{userName}/{User.Password}"));
                    if (hasRight)
                    {
                        await ApiManager.Change($"api/chat/renameUser/{newUserName}", $"{{ 'Name':'{userName}' }}");

                        signalRManager.RenameUser(userName, newUserName);
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
        private async void BanUser()
        {
            if (Command["specialCommand"] == "-m" &&
               Command["valueSpecialCommand"] != string.Empty &&
               Command["objectName"] != string.Empty)
            {
                bool hasRight = Convert.ToBoolean(await ApiManager.Read($"api/chat/hasLowRightInChat/allChats/{User.Name}"));
                if (hasRight)
                {
                    MainWindow mainWindow = new MainWindow();
                    SignalRManager signalRManager = new SignalRManager();
                    double time = Convert.ToDouble(Command["valueSpecialCommand"]);
                    string userBannedName = Command["objectName"];

                    await ApiManager.Change($"api/chat/banUser/{time}", $"{{ 'Name':'{userBannedName}'}} ");

                    signalRManager.BanUserToChat("allChats", userBannedName);
                }
                else
                    MessageBox.Show("У вас нет прав на это действие.");
            }
            else
                MessageBox.Show("Некорректное(-ая) имя/команда.");
        }
        private async void Moderator()
        {
            if ((Command["specialCommand"] == "-n" || Command["specialCommand"] == "-d") &&
               Command["objectName"] != string.Empty)
            {
                string moderatorName = Command["objectName"];
                if (moderatorName != User.Name)
                {
                    SignalRManager signalRManager = new SignalRManager();
                    bool isModerator = false;

                    if (Command["specialCommand"] == "-n")
                        isModerator = true;
                    else if (Command["specialCommand"] == "-d")
                        isModerator = false;

                    await ApiManager.Change($"api/chat/changeModerator/{isModerator}", $"{{ 'Name':'{moderatorName}'}} ");

                    signalRManager.ChangeRoleUser();
                }
                else
                    MessageBox.Show("Нельзя самостоятельно назначать себя модератором.");
            }
            else
                MessageBox.Show("Некорректное(-ая) имя/команда.");
        }

        private void BotAction()
        {
            switch (Command["action"])
            {
                case "find":
                    Find();

                    break;
                default:
                    MessageBox.Show("Некорректная команда.");
                    break;
            }
        }
        private async void Find()
        {
            if (Command["specialCommand"] == "-t" &&
                Command["valueSpecialCommand"] != string.Empty &&
                Command["objectName"] != string.Empty &&
                ChatSelected != null)
            {
                try
                {
                    SignalRManager signalRManager = new SignalRManager();
                    YouTubeManager youTubeManager = new YouTubeManager();

                    string channelName = Command["objectName"];
                    string videoName = Command["valueSpecialCommand"];

                    await youTubeManager.SetIdAndUrlVideo(videoName, channelName);

                    signalRManager.SendMessage(ChatSelected, "yBot", youTubeManager.Url);
                    ApiManager.Create("api/chat/message", $"{{\"ChatName\":\"{ChatSelected}\",\"Author\":\"yBot\",\"Text\":\"{youTubeManager.Url}\"}}");

                    if (Command["secondSpecialCommand"] == "-v")
                    {
                        youTubeManager.SetVideoInfo();
                        string viewsMessageLine = "Просмотров: " + youTubeManager.ViewCount.ToString();

                        signalRManager.SendMessage(ChatSelected, "yBot", viewsMessageLine);
                        ApiManager.Create("api/chat/message", $"{{\"ChatName\":\"{ChatSelected}\",\"Author\":\"yBot\",\"Text\":\"{viewsMessageLine}\"}}");
                    }
                    else if (Command["secondSpecialCommand"] == "-l")
                    {
                        youTubeManager.SetVideoInfo();
                        string likesMessageLine = "Лайков: " + youTubeManager.LikeCount.ToString();

                        signalRManager.SendMessage(ChatSelected, "yBot", likesMessageLine);
                        ApiManager.Create("api/chat/message", $"{{\"ChatName\":\"{ChatSelected}\",\"Author\":\"yBot\",\"Text\":\"{likesMessageLine}\"}}");
                    }
                }
                catch
                {
                    MessageBox.Show("Видео не найдено.");
                }
            }
            else
                MessageBox.Show("Некорректная команда.");
        }

        #region WriteConsoleLineToCommandDictionary-methods
        private void WriteConsoleLineToCommand()
        {
            try
            {
                string[] specialCommands = {
                    "-c",
                    "-l",
                    "-m",
                    "-n",
                    "-d",
                    "-t",
                    "-v",
                };
                
                string[] words = Regex.Split(ConsoleLine, " ");
                int countCommand = 0;

                for (int i = 0; i < words.Length; i++)
                {
                    string currentCommand = specialCommands.Where(w => w == words[i]).FirstOrDefault();
                    if (currentCommand != null)
                    {
                        AddCommand(ref countCommand, words[i].ToLower());
                        i++;
                        AddValuesToSpecialCommand(ref countCommand, ref i, words);
                    }
                    else
                        AddCommand(ref countCommand, words[i].ToLower());
                }
            }
            catch
            {
                MessageBox.Show("Проверьте набранную команду.");
            }
        }
        private void AddValuesToSpecialCommand(ref int countCommand, ref int i, string[] words)
        {
            for (int j = i; j < words.Length; j++)
            {
                if (!words[j].Contains("-"))
                    Command[Command.ElementAt(countCommand).Key] += words[j];
                else
                    break;

                if (j + 1 < words.Length && !words[j + 1].Contains("-"))
                    Command[Command.ElementAt(countCommand).Key] += " ";

                i = j;
            }
            countCommand++;
        }
        private void AddCommand(ref int countCommand, string word)
        {
            Command[Command.ElementAt(countCommand).Key] = word;
            countCommand++;
        }
        #endregion
    }
}