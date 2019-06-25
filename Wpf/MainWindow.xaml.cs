using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Wpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static HubConnection connection;

        public MainWindow()
        {
            InitializeComponent();

            if (connection == null)
            {
                connection = new HubConnectionBuilder()
                    .WithUrl("http://localhost:58269/ChatHub")
                    .Build();
            }

            //connection.Closed += async (error) =>
            //{
            //    await Task.Delay(new Random().Next(0, 5) * 1000);
            //    await connection.StartAsync();
            //};


            //Thread secondThread = new Thread(SecondThread);
            //secondThread.Start();
        }

        public async static void MessageButton_Click(object sender, EventArgs e)
        {
            MainWindow mainWindow = new MainWindow();

            mainWindow.Send();

            if (ChatControl.TextBox[ChatSelected].Text != string.Empty)
                ApiManager.Create("api/chat/message", $"{{\"ChatName\":\"{ChatSelected}\",\"Author\":\"{User.Name}\",\"Text\":\"{ChatControl.TextBox[ChatSelected].Text}\"}}");

            ChatControl.TextBox[ChatSelected].Clear();
        }

        #region Methods update boxes and chat tabs. For debudding using button, not thread
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Thread secondThread = new Thread(SecondThread);
            secondThread.Start();
        }
        private async void SecondThread()
        {
            await Dispatcher.BeginInvoke((Action)(() => UpdateChatTabs()));
            Dispatcher.BeginInvoke((Action)(() => UpdateListBoxes()));

            Thread.Sleep(300);
            //SecondThread();
        }
        public async void UpdateUsersBox()
        {
            if (ChatSelected != null)
            {
                List<string> users = new List<string>();

                string usersJson = await ApiManager.Read($"api/chat/users/{ChatSelected}");

                users = GetListValuesFromJson(usersJson, "name");

                ChatControl.UsersBox[ChatSelected].Items.Clear();
                foreach (string user in users)
                    ChatControl.UsersBox[ChatSelected].Items.Add(user);
            }
        }
        private async void UpdateListBoxes()
        {
            //if (chatControl.SelectedItem != null)
            //{
                List<string> users = new List<string>();
                List<string> authors = new List<string>();
                List<string> texts = new List<string>();
                
                string usersJson = await ApiManager.Read($"api/chat/users/{ChatSelected}");
                string messagesJson = await ApiManager.Read($"api/chat/messages/{ChatSelected}");
                
                users = GetListValuesFromJson(usersJson, "name");
                authors = GetListValuesFromJson(messagesJson, "author");
                texts = GetListValuesFromJson(messagesJson, "text");

                ChatControl.ChatBox[ChatSelected].Items.Clear();
                for (int i = 0; i < authors.Count; i++)
                    if (authors[i] != string.Empty && texts[i] != string.Empty)
                        ChatControl.ChatBox[ChatSelected].Items.Add(authors[i] + ": " + texts[i]);

                ChatControl.UsersBox[ChatSelected].Items.Clear();
                foreach (string user in users)
                    ChatControl.UsersBox[ChatSelected].Items.Add(user);
                
            //}
        }
        private async void UpdateChatTabs()
        {
            if (ChatsControl.HasItems)
            {
                List<string> chats = new List<string>();
                bool hasChat = false;

                string chatsJson = await ApiManager.Read($"api/chat/chats");
                chats = GetListValuesFromJson(chatsJson, "name");

                foreach (string chat in chats)
                    if (chat == ChatSelected)
                        hasChat = true;

                if (!hasChat)
                {
                    ChatControl chatWindow = new ChatControl(ChatsControl);
                    chatWindow.DeleteTabItem(ChatSelected);
                }
            }
        }
        #endregion

        private async void ExitUserItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public static string ChatSelected;
        private void ChatsControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChatsControl.SelectedItem != null)
                ChatSelected = ((TabItem)ChatsControl.SelectedItem).Name;
        }

        private void ConsoleBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string consoleLine = ConsoleBox.Text;

                СonsoleManager console = new СonsoleManager(ChatsControl, consoleLine);
                console.ExecuteCommand();

                ConsoleBox.Clear();
            }
        }
        #region ToolTip info
        private void ConsoleExpander_MouseMove(object sender, MouseEventArgs e)
        {
            ConsoleExpander.ToolTip = @"
                Комнаты:
                  •//room create { Название комнаты } — создает комнаты
                     -c закрытая комната. Только(владелец, модератор и админ) сможет добавлять/ удалять пользоватей из комнаты
                  •//room remove { Название комнаты } — удаляет комнату (владелец и админ)
                  •//room rename { Название комнаты } — переименование комнаты (владелец и админ)
                  •//room connect { Название комнаты } — войти в комнату
                     -l { login пользователя } — добавить пользователя в комнату
                  •//room disconnect — выйти из текущей комнаты
                  •//room disconnect { Название комнаты } — выйти из заданной комнаты
                     -l { login пользователя } — выгоняет пользователя из комнаты(для владельца, модератора и админа)
                     -m { Колличество минут } — время на которое пользователь не сможет войти(для владельца, модератора и админа)
            Пользователи:
                  •//user rename { login пользователя } (владелец и админ)
                  •//user ban
                    -l { login пользователя } — выгоняет пользователя из всех комнат
                    -m { Колличество минут } — время на которое пользователь не сможет войти
                  •//user moderator { login пользователя } — действия над модераторами
                    -n — назначить пользователя модератором
                    -d — разжаловать пользователя
            Боты:
                  •//yBot find -k -l { название канала }||{ название видео } - в ответ бот присылает ссылку на ролик
                    -v — выводит колличество текущих просмотров
                    -l — выводит колличество лаqков под видео
                  •//yBot help — список доступных команд для взаимодействи
            Другие:
                  •//help - выводит список доступных команд
            ";
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

        #region SignalRMethods
        public async void Send()
        {
            await connection.InvokeAsync("SendMessage", User.Name, ChatControl.TextBox[ChatSelected].Text);
        }

        public async void OnConnect()
        {
            connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var newMessage = $"{user}: {message}";
                    if (ChatControl.ChatBox.Count != 0)
                        ChatControl.ChatBox[ChatSelected].Items.Add(newMessage);
                });
            });
            connection.On<string, string>("ReceiveUser", (chatName, userName) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var newUser = $"{userName}";
                    if (ChatControl.UsersBox.Count != 0)
                        ChatControl.UsersBox[chatName].Items.Add(newUser);
                });
            });
            connection.On<string, string>("RemoveUser", (chatName, userName) =>
            {
                Dispatcher.Invoke(() =>
                {
                    UpdateUsersBox();
                });
            });
            connection.On<string, string>("BanUser", (chatName, userName) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var userBanned = $"{userName}";
                    if (User.Name == userName)
                    {
                        ChatControl chatWindow = new ChatControl(ChatsControl);
                        chatWindow.DeleteTabItem(chatName);
                    }
                });
            });


            await connection.StartAsync();
        }
        public async Task OnDisconnect()
        {
            await connection.StopAsync();
            connection = null;
        }

        public async void AddUserToChat(string chatName)
        {
            await connection.InvokeAsync("AddUserToChat", chatName, User.Name);
        }

        public async void RemoveUserFromChat(string chatName, string userName)
        {
            await connection.InvokeAsync("RemoveUserFromChat", chatName, userName);
        }

        public async void BanUserToChat(string chatName, string userName)
        {
            await connection.InvokeAsync("BanUserToChat", chatName, userName);
        }

        private async void Window_Closed(object sender, EventArgs e)
        {
            AuthorizationWindow authorizationWindow = new AuthorizationWindow();

            await ApiManager.Change($"api/authorization/logout", $"{{'Name':'{User.Name}', 'Password':'{User.Password}'}}");

            OnDisconnect();
            Close();
            authorizationWindow.Show();
        }
        #endregion
    }
}