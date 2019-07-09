using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.AspNetCore.SignalR.Client;

namespace Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (SignalRManager.connection == null)
            {
                SignalRManager.connection = new HubConnectionBuilder()
                    .WithUrl(SignalRManager.urlSignalR + SignalRManager.uriSignalR)
                    .Build();
                SignalRManager.ChatsControl = ChatsControl;

                LoadPreviouslyOpenedChatsItems();
            }
        }

        private async void LoadPreviouslyOpenedChatsItems()
        {
            ChatControl chatControl = new ChatControl(ChatsControl);
            List<string> chatsNames = new List<string>();

            string chatsJson = await ApiManager.Read($"api/chat/getChats/{User.Name}");
            chatsNames = GetListValuesFromJson(chatsJson, "name");

            foreach (string chatName in chatsNames)
            {
                chatControl.AddTabItem(chatName);
                await UpdateUsersBox();
            }
        }

        public async Task UpdateUsersBox()
        {
            if (ChatSelected != null)
            {
                List<string> usersNames = new List<string>();

                string usersJson = await ApiManager.Read($"api/chat/getUsers/{ChatSelected}");
                usersNames = GetListValuesFromJson(usersJson, "name");

                ChatControl.UsersBox[ChatSelected].Items.Clear();
                foreach (string user in usersNames)
                    ChatControl.CreateListBoxItem(ChatSelected, user);
            }
        }

        public static string ChatSelected;
        private void ChatsControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChatsControl.SelectedItem != null)
                ChatSelected = ((TabItem)ChatsControl.SelectedItem).Name;
        }

        private void ExitUserItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void Window_Closed(object sender, EventArgs e)
        {
            AuthorizationWindow authorizationWindow = new AuthorizationWindow();
            SignalRManager signalRManager = new SignalRManager();

            await ApiManager.Change($"api/authorization/logout", $"{{'Name':'{User.Name}', 'Password':'{User.Password}'}}");
            signalRManager.OnDisconnect();

            Close();
            authorizationWindow.Show();
        }

        public static void MessageButton_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (ConsoleBox.IsSelectionActive)
                {
                    string consoleLine = ConsoleBox.Text;

                    СonsoleManager console = new СonsoleManager(ChatsControl, consoleLine);
                    console.ExecuteCommand();

                    ConsoleBox.Clear();
                }
                else if (ChatControl.TextBox[ChatSelected].IsSelectionActive)
                    SendMessage();
            }
        }

        public static void SendMessage()
        {
            MainWindow mainWindow = new MainWindow();
            SignalRManager signalRManager = new SignalRManager();

            string chatName = ChatSelected;
            string userName = User.Name;
            string message = ChatControl.TextBox[ChatSelected].Text;

            if (message != string.Empty)
            {
                signalRManager.SendMessage(chatName, userName, message);
                ApiManager.Create("api/chat/message", $"{{\"ChatName\":\"{chatName}\",\"Author\":\"{userName}\",\"Text\":\"{message}\"}}");
            }

            ChatControl.TextBox[chatName].Clear();
        }

        #region ToolTip info
        private void ConsoleExpander_MouseMove(object sender, MouseEventArgs e)
        {
            ConsoleExpander.ToolTip = @"
                Комнаты:
                  •room create { Название комнаты } — создает комнаты
                     -c закрытая комната. Только(владелец, модератор и админ) сможет добавлять/ удалять пользоватей из комнаты
                  •room remove { Название комнаты } — удаляет комнату (владелец и админ)
                  •room rename { Название комнаты } — переименование комнаты (владелец и админ)
                  •room connect { Название комнаты } — войти в комнату
                     -l { login пользователя } — добавить пользователя в комнату
                  •room disconnect — выйти из текущей комнаты
                  •room disconnect { Название комнаты } — выйти из заданной комнаты
                     -l { login пользователя } — выгоняет пользователя из комнаты(для владельца, модератора и админа)
                     -m { Колличество минут } — время на которое пользователь не сможет войти(для владельца, модератора и админа)
                Пользователи:
                  •user rename { login пользователя } — сменить свой логи (для владелеца)
                  •user rename { login пользователя }  — переименовать другого пользователя (для владелеца и админа)
                     -l { новый login пользователя } — новый логин пользователя (для владелеца и админа)
                  •user ban { login пользователя } — выгоняет пользователя из всех комнат(для модератора и админа)
                    -m { Колличество минут } — время на которое пользователь не сможет войти(для модератора и админа)
                  •user moderator { login пользователя } — действия над модераторами
                    -n — назначить пользователя модератором
                    -d — разжаловать пользователя
                Боты:
                  •yBot find { название канала без пробелов } -t { название видео } - в ответ бот присылает ссылку на ролик
                    -v — выводит колличество текущих просмотров
                    -l — выводит колличество лайков под видео
                  •yBot help — список доступных команд для взаимодействи

                Пример:
                  room disconnect newroom -l userlogin -m 60
            ";
        }

        public static void UsersBox_MouseMove(object sender, MouseEventArgs e)
        {
            ChatControl.UsersBox[ChatSelected].ToolTip = @"Создатель комнаты — зеленый
Администратор — золотой
Модератор — оранжевый
Пользователь — черный
Забаненный пользователь — красный";
        }
        #endregion

        #region Methods converting Json to list string
        private List<string> GetListValuesFromJson(string json, string attribute)
        {
            List<string> values = new List<string>();

            json = json.Trim(new char[] { '[', ']' });
            string[] jsonLines = Regex.Split(json, "},{");

            foreach (string jsonLine in jsonLines)
                values.Add(GetValueByAttribute(jsonLine, attribute));

            return values;
        }
        private string GetValueByAttribute(string jsonLine, string attribute)
        {
            string value = string.Empty;

            if (jsonLine != "")
            {
                List<string> words = new List<string>();

                jsonLine = jsonLine.Trim(new char[] { '{', '}' });
                string[] symbolWords = Regex.Split(jsonLine, "\"");

                for (int i = 0; i < symbolWords.Length; i++)
                    if (symbolWords[i] != ":" && symbolWords[i] != ":null," && symbolWords[i] != "," && symbolWords[i] != "")
                        words.Add(symbolWords[i]);

                for (int i = 0; i < words.Count; i++)
                    if (words[i] == attribute && words[i + 1] != "message" && words[i + 1] != "userNames")
                        value = words[++i];
            }

            return value;
        }
        #endregion
    }
}