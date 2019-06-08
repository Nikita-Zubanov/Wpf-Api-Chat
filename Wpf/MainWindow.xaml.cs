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
        }

        private Connection connect;

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            connect = Connection.GetConnection();            
            List<string> users = new List<string>();

            if (connect.IsConnected)
            {
                await ApiManager.Delete("api/users", connect.UserName);
                connect.Disconnecting();

                UsersBox.Items.Clear();
                ChatBox.Items.Clear();
            }
            else
            {
                connect.Connecting(UserBox.Text);
                await ApiManager.Create("api/users", "{\"Name\":\"" + connect.UserName + "\"}");

                var a = ApiManager.Read("api/users");

                users = GetListValuesFromJson(await a, "name");
            }

            foreach (string user in users)
                UsersBox.Items.Add(user);
        }

        private async void MessageButton_Click(object sender, RoutedEventArgs e)
        {
            connect = Connection.GetConnection();
            List<string> authors = new List<string>();
            List<string> messages = new List<string>();

            if (connect.IsConnected)
            {
                await ApiManager.Create("api/chat", "{\"Author\":\"" + connect.UserName + "\",\"Message\":\"" + TextBox.Text + "\"}"); // переделать {0}

                messages = GetListValuesFromJson(await ApiManager.Read("api/chat"), "message");
                authors = GetListValuesFromJson(await ApiManager.Read("api/chat"), "author");
            }

            ChatBox.Items.Clear();
            for (int i = 0; i < authors.Count; i++)
                ChatBox.Items.Add(authors[i] + ": " + messages[i]);
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
