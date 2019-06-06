using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            connect = Connection.GetConnection();
            List<string> users = new List<string>();

            if (connect.IsConnected)
            {
                CRUD.Delete("api/users", connect.UserName);
                connect.Disconnecting();

                UsersBox.Items.Clear();
                ChatBox.Items.Clear();
            }
            else
            {
                connect.Connecting(UserBox.Text);
                CRUD.Create("api/users", "{\"Name\":\"" + connect.UserName + "\"}");

                users = GetListValuesFromJson(CRUD.Read("api/users").Result, "name");
            }

            foreach (string user in users)
                UsersBox.Items.Add(user);
        }

        private void MessageButton_Click(object sender, RoutedEventArgs e)
        {
            
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
