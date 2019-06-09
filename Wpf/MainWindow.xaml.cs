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

                string usersJson = await ApiManager.Read("api/users");
                string chatJson = await ApiManager.Read($"api/chat/{ChatSelected}");

                users = GetListValuesFromJson(usersJson, "name");
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

        private List<string> GetListValuesFromJson(string jsonLine, string attribute)
        {
            string[] lines = Regex.Split(jsonLine, "},{");
            List<string> values = new List<string>();

            foreach (string line in lines)
            {
                values.Add(GetValueByAttribute(line, attribute));
            }

            return values;
        }
        private string GetValueByAttribute(string line, string attribute)
        {
            string[] words = Regex.Split(line, "\"");
            string value = "";

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i] == attribute)
                {
                    i += 2;
                    value = words[i];
                }
            }

            return value;
        }
    }
}
