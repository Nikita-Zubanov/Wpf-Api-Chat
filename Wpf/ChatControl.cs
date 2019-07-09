using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Wpf
{
    class ChatControl
    {
        private TabControl ChatsControl;
        private static Dictionary<string, TabItem> ChatsItems = new Dictionary<string, TabItem>();

        public static Dictionary<string, Button> MessageButton = new Dictionary<string, Button>();
        public static Dictionary<string, ListBox> ChatBox = new Dictionary<string, ListBox>();
        public static Dictionary<string, TextBox> TextBox = new Dictionary<string, TextBox>();
        public static Dictionary<string, ListBox> UsersBox = new Dictionary<string, ListBox>();

        public ChatControl(TabControl chatControl)
        {
            ChatsControl = chatControl;
        }

        public void AddTabItem(string tabName)
        {
            if (ChatsItems.ContainsKey(tabName))
            {
                DeleteTabItem(tabName);
                CreateTabItem(tabName);
            }
            else
                CreateTabItem(tabName);

            ChatsControl.Items.Add(ChatsItems[tabName]);
            ChatsControl.SelectedIndex = ChatsControl.Items.Count - 1;
        }

        private void CreateTabItem(string chatName)
        {
            ChatBox.Add(chatName, new ListBox
            {
                Name = chatName,
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = 272,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 617
            });
            TextBox.Add(chatName, new TextBox
            {
                Name = chatName,
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = 50,
                Margin = new Thickness(0, 277, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Width = 617,
                FontFamily = new FontFamily("Segoe UI Semibold")
            });
            UsersBox.Add(chatName, new ListBox
            {
                Name = chatName,
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = 272,
                Margin = new Thickness(622, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Width = 144
            });
            UsersBox[chatName].MouseMove += MainWindow.UsersBox_MouseMove;
            MessageButton.Add(chatName, new Button
            {
                Name = chatName,
                Content = "Отправить",
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(622, 277, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Width = 75
            });
            MessageButton[chatName].Click += MainWindow.MessageButton_Click;


            Grid chatGrid = new Grid();
            chatGrid.Name = "ChatGrid";
            chatGrid.Background = Brushes.WhiteSmoke;
            chatGrid.Children.Add(ChatBox[chatName]);
            chatGrid.Children.Add(TextBox[chatName]);
            chatGrid.Children.Add(UsersBox[chatName]);
            chatGrid.Children.Add(MessageButton[chatName]);

            ChatsItems.Add(chatName,
                new TabItem()
                {
                    Name = chatName,
                    Header = chatName,
                    Margin = new Thickness(-2, -2, 2, 0),
                    Content = chatGrid
                });
        }

        public void DeleteTabItem(string chatName)
        {
            if (ChatsItems.ContainsKey(chatName))
            {
                ChatsControl.Items.Remove(ChatsItems[chatName]);
                ChatsItems.Remove(chatName);
                MessageButton.Remove(chatName);
                ChatBox.Remove(chatName);
                TextBox.Remove(chatName);
                UsersBox.Remove(chatName);
            }
        }

        public void RenameTabItem(string oldName, string newName)
        {
            foreach (KeyValuePair<string, TabItem> keyValuePair in ChatsItems)
                if (keyValuePair.Key == oldName)
                {
                    ChatsItems[keyValuePair.Key].Name = newName;
                    ChatsItems[keyValuePair.Key].Header = newName;
                    TabItem tabItem = ChatsItems[keyValuePair.Key];
                    ChatsItems.Remove(keyValuePair.Key);
                    ChatsItems.Add(newName, tabItem);

                    MessageButton[keyValuePair.Key].Name = newName;
                    Button button = MessageButton[keyValuePair.Key];
                    MessageButton.Remove(keyValuePair.Key);
                    MessageButton.Add(newName, button);

                    ChatBox[keyValuePair.Key].Name = newName;
                    ListBox chatBox = ChatBox[keyValuePair.Key];
                    ChatBox.Remove(keyValuePair.Key);
                    ChatBox.Add(newName, chatBox);

                    TextBox[keyValuePair.Key].Name = newName;
                    TextBox textBox = TextBox[keyValuePair.Key];
                    TextBox.Remove(keyValuePair.Key);
                    TextBox.Add(newName, textBox);

                    UsersBox[keyValuePair.Key].Name = newName;
                    ListBox usersBox = UsersBox[keyValuePair.Key];
                    UsersBox.Remove(keyValuePair.Key);
                    UsersBox.Add(newName, usersBox);

                    break;
                }
        }

        public void DeleteAllTabItem()
        {
            ChatsControl.Items.Clear();
            ChatsItems.Clear();
            MessageButton.Clear();
            ChatBox.Clear();
            TextBox.Clear();
            UsersBox.Clear();
        }

        public static void UpdateListBoxItem(string chatName, string userName)
        {
            foreach (ListBoxItem item in UsersBox[chatName].Items)
                if (item.Content.ToString() == userName)
                {
                    UsersBox[chatName].Items.Remove(item);
                    break;
                }

            CreateListBoxItem(chatName, userName);
        }

        public static async void CreateListBoxItem(string chatName, string userName)
        {
            SolidColorBrush userColorText = new SolidColorBrush();

            string status = await ApiManager.Read($"api/chat/statusUser/{chatName}/{userName}");
            bool isBanned = Convert.ToBoolean(await ApiManager.Read($"api/chat/isUserBanned/{chatName}/{userName}"));
            switch (status)
            {
                case User.creator:
                    userColorText = User.creatorColor;
                    break;
                case User.administrator:
                    userColorText = User.administratorColor;
                    break;
                case User.moderator:
                    userColorText = User.moderatorColor;
                    break;
                case User.user:
                    userColorText = User.userColor;
                    break;
            }
            if (isBanned)
                userColorText = User.bannedColor;

            ListBoxItem newListBoxItem = new ListBoxItem
            {
                Content = userName,
                Foreground = userColorText,
                FontFamily = new FontFamily("Segoe UI Semibold")
        };
            UsersBox[chatName].Items.Add(newListBoxItem);
        }

        public static bool HasUserToUsersBox(string tabName, string userName)
        {
            foreach (ListBoxItem item in UsersBox[tabName].Items)
                if (item.Content.ToString() == userName)
                    return true;

            return false;
        }

        public static bool HasTabItemToDictionary(string tabName)
        {
            if (ChatsItems.ContainsKey(tabName))
                return true;

            return false;
        }
    }
}