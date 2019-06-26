using System;
using System.Windows;

namespace Wpf
{
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
                bool isRegistred = Convert.ToBoolean(await ApiManager.Read($"api/authorization/{User.Name}/{User.Password}"));
                if (isRegistred)
                {
                    MainWindow mainWindow = new MainWindow();

                    ApiManager.Change("api/authorization/login", $"{{'Name':'{User.Name}', 'Password':'{User.Password}'}}");
                    mainWindow.OnConnect();

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
                mainWindow.OnConnect();

                Close();
                mainWindow.ShowDialog();
            }
            else
                MessageBox.Show("Заполните все поля!");
        }
    }
}