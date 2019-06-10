using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Wpf
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        public AuthorizationWindow()
        {
            InitializeComponent();
        }

        private async void AuthorizeButton_Click(object sender, RoutedEventArgs e)
        {
            User.Name = UserNameBox.Text;
            User.Password = UserPasswordBox.Text;

            if (User.Name != string.Empty && User.Password != string.Empty)
            {
                MainWindow mainWindow = new MainWindow();

                bool isRegistred = Convert.ToBoolean(await ApiManager.Read($"api/authorization/{User.Name}/{User.Password}"));
                if (isRegistred)
                {
                    ApiManager.Change("api/authorization/login", $"{{'Name':'{User.Name}', 'Password':'{User.Password}'}}");

                    Close();
                    mainWindow.ShowDialog();
                }
                else
                    MessageBox.Show("Вы не зарегестрированы!");

                UserNameBox.Clear();
                UserPasswordBox.Clear();
            }
            else
                MessageBox.Show("Заполните все поля!");
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            User.Name = UserNameBox.Text;
            User.Password = UserPasswordBox.Text;

            if (User.Name != string.Empty && User.Password != string.Empty)
            {
                MainWindow mainWindow = new MainWindow();
                
                ApiManager.Create("api/authorization/register", $"{{'Name':'{User.Name}', 'Password':'{User.Password}'}}");

                Close();
                mainWindow.ShowDialog();
            }
            else
                MessageBox.Show("Заполните все поля!");
        }
    }
}
