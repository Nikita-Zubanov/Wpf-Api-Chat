﻿using System;
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
                bool isRegistred = Convert.ToBoolean(await ApiManager.Read($"api/authorization/isRegistred/{User.Name}/{User.Password}"));
                if (isRegistred)
                {
                    MainWindow mainWindow = new MainWindow();
                    SignalRManager signalRManager = new SignalRManager();

                    ApiManager.Change("api/authorization/login", $"{{'Name':'{User.Name}', 'Password':'{User.Password}'}}");
                    signalRManager.OnConnect();

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

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            User.Name = UserNameBox.Text;
            User.Password = UserPasswordBox.Text;

            if (User.Name != string.Empty && User.Password != string.Empty)
            {
                if (IsAllowedName())
                {
                    bool isUserExists = Convert.ToBoolean(await ApiManager.Read($"api/chat/isUserExists/{User.Name}"));
                    if (!isUserExists)
                    {
                        MainWindow mainWindow = new MainWindow();
                        SignalRManager signalRManager = new SignalRManager();

                        ApiManager.Create("api/authorization/register", $"{{'Name':'{User.Name}', 'Password':'{User.Password}'}}");
                        signalRManager.OnConnect();

                        Close();
                        mainWindow.ShowDialog();
                    }
                    else
                        MessageBox.Show("Пользователь с таким логином уже существует.");
                }
                else
                    MessageBox.Show("Предложенный логин запрещен.");
            }
            else
                MessageBox.Show("Заполните все поля!");
        }

        private bool IsAllowedName()
        {
            string[] statusWords = { User.banned, User.user, User.creator, User.administrator, User.moderator };

            foreach (string statusWord in statusWords)
                if (User.Name.Contains(statusWord))
                    return false;

            return true;
        }
    }
}