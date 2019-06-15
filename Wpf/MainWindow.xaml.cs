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

namespace Wpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Thread secondThread = new Thread(SecondThread);
            //secondThread.Start();
        }

        public static void MessageButton_Click(object sender, EventArgs e)
        {
            if (ChatWindow.TextBox[ChatSelected].Text != string.Empty)
                ApiManager.Create("api/chat/message", $"{{\"ChatName\":\"{ChatSelected}\",\"Author\":\"{User.Name}\",\"Text\":\"{ChatWindow.TextBox[ChatSelected].Text}\"}}");

            ChatWindow.TextBox[ChatSelected].Clear();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Thread secondThread = new Thread(SecondThread);
            secondThread.Start();
        }
        private void SecondThread()
        {
            Dispatcher.BeginInvoke((Action)(() => UpdateListBoxes()));

            Thread.Sleep(300);
            //SecondThread();
        }
        private async void UpdateListBoxes()
        {
            if (ChatsControl.HasItems)
            {
                List<string> users = new List<string>();
                List<string> authors = new List<string>();
                List<string> messages = new List<string>();

                string usersJson = await ApiManager.Read($"api/chat/users/{ChatSelected}");
                string chatJson = await ApiManager.Read($"api/chat/{ChatSelected}");

                users = GetListValuesFromJson(usersJson, "name");
                authors = GetListValuesFromJson(chatJson, "author");
                messages = GetListValuesFromJson(chatJson, "text");
                
                ChatWindow.ChatBox[ChatSelected].Items.Clear();
                for (int i = 0; i < authors.Count; i++)
                    if (authors[i] != string.Empty && messages[i] != string.Empty)
                        ChatWindow.ChatBox[ChatSelected].Items.Add(authors[i] + ": " + messages[i]);

                ChatWindow.UsersBox[ChatSelected].Items.Clear();
                foreach (string user in users)
                    ChatWindow.UsersBox[ChatSelected].Items.Add(user);
            }
        }

        private void ExitUserItem_Click(object sender, RoutedEventArgs e)
        {
            AuthorizationWindow authorizationWindow = new AuthorizationWindow();

            ApiManager.Change($"api/authorization/logout", $"{{'Name':'{User.Name}', 'Password':'{User.Password}'}}");

            Close();
            authorizationWindow.Show();
        }

        private void CreateChatItem_Click(object sender, RoutedEventArgs e)
        {
            ChatWindow creatingWindow = new ChatWindow(ChatsControl);

            creatingWindow.Show();
        }

        private static string ChatSelected;
        private void ChatsControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChatSelected = ((TabItem)ChatsControl.SelectedItem).Name;
        }

        private void ConsoleBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MessageBox.Show(ConsoleBox.Text);

                ConsoleBox.Clear();
            }
        }

        private void ExpanderConsoleBox_MouseMove(object sender, MouseEventArgs e)
        {
            #region
            ExpanderConsoleBox.ToolTip = @"
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
            #endregion
        }

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
            List<string> words = new List<string>();
            string value = "";

            jsonLine = jsonLine.Trim(new char[] { '{', '}' });
            string[] symbolWords = Regex.Split(jsonLine, "\"");

            for (int i = 0; i < symbolWords.Length; i++)
                if (symbolWords[i] != ":" && symbolWords[i] != ":null," && symbolWords[i] != "," && symbolWords[i] != "")
                    words.Add(symbolWords[i]);

            for (int i = 0; i < words.Count; i++)
                if (words[i] == attribute && words[i + 1] != "message" && words[i + 1] != "userNames")
                    value = words[++i];

            return value;
        }
    }
}
