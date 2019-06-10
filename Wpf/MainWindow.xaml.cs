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

            Thread secondThread = new Thread(SecondThread);
            secondThread.Start();
        }

        public static void MessageButton_Click(object sender, EventArgs e)
        {
            if (ChatWindow.TextBox[ChatSelected].Text != string.Empty)
                ApiManager.Create("api/chat/message", $"{{\"Name\":\"{ChatSelected}\",\"Author\":\"{User.Name}\",\"Message\":\"{ChatWindow.TextBox[ChatSelected].Text}\"}}");
        }

        private void SecondThread()
        {
            Dispatcher.BeginInvoke((Action)(() => UpdateListBoxes()));

            Thread.Sleep(500);
            SecondThread();
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

                users = GetListValuesFromJson(usersJson, "nameUser");
                authors = GetListValuesFromJson(chatJson, "author");
                messages = GetListValuesFromJson(chatJson, "message");

                ChatWindow.UsersBox[ChatSelected].Items.Clear();
                foreach (string user in users)
                    ChatWindow.UsersBox[ChatSelected].Items.Add(user);

                ChatWindow.ChatBox[ChatSelected].Items.Clear();
                for (int i = 0; i < authors.Count; i++)
                    if (authors[i] != string.Empty && messages[i] != string.Empty)
                        ChatWindow.ChatBox[ChatSelected].Items.Add(authors[i] + ": " + messages[i]);
            }
        }

        private void ExitUserItem_Click(object sender, RoutedEventArgs e)
        {
            AuthorizationWindow authorizationWindow = new AuthorizationWindow();

            ApiManager.Change($"api/authorization/{User.Name}", $"{{'Name':'{User.Name}', 'Password':'{User.Password}'}}");

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
