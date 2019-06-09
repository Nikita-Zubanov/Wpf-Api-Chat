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
            if (CreatingChatWindow.TextBox.Text != string.Empty)
                ApiManager.Create("api/chat", $"{{\"Author\":\"{User.Name}\",\"Message\":\"{CreatingChatWindow.TextBox.Text}\"}}");
        }

        private void SecondThread()
        {
            Dispatcher.BeginInvoke((Action)(() => UpdateListBoxes()));

            Thread.Sleep(500);
            SecondThread();
        }

        private async void UpdateListBoxes()
        {
            List<string> users = new List<string>();
            List<string> authors = new List<string>();
            List<string> messages = new List<string>();

            string usersJson = await ApiManager.Read("api/users");
            string chatJson = await ApiManager.Read("api/chat");

            users = GetListValuesFromJson(usersJson, "name");
            authors = GetListValuesFromJson(chatJson, "author");
            messages = GetListValuesFromJson(chatJson, "message");

            UsersBox.Items.Clear();
            foreach (string user in users)
                UsersBox.Items.Add(user);

            ChatBox.Items.Clear();
            for (int i = 0; i < authors.Count; i++)
                if (authors[i] != string.Empty && messages[i] != string.Empty)
                    ChatBox.Items.Add(authors[i] + ": " + messages[i]);
        }

        private void ExitUserItem_Click(object sender, RoutedEventArgs e)
        {
            AuthorizationWindow authorizationWindow = new AuthorizationWindow();
            User.Status = "Offline";

            ApiManager.Change($"api/authorization/{User.Name}", $"{{'Name':'{User.Name}', 'Password':'{User.Password}', 'Status':'{User.Status}'}}");

            Close();
            authorizationWindow.Show();
        }

        private void CreateChatItem_Click(object sender, RoutedEventArgs e)
        {
            CreatingChatWindow creatingWindow = new CreatingChatWindow(ChatsControl);

            creatingWindow.Show();
        }

        private string ChatSelected;
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
