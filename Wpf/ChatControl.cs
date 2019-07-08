using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Wpf
{
    class ChatControl
    {
        private TabControl TabControl;
        private static Dictionary<string, TabItem> TabItems = new Dictionary<string, TabItem>();

        public static Dictionary<string, Button> MessageButton = new Dictionary<string, Button>();
        public static Dictionary<string, ListBox> ChatBox = new Dictionary<string, ListBox>();
        public static Dictionary<string, TextBox> TextBox = new Dictionary<string, TextBox>();
        public static Dictionary<string, ListBox> UsersBox = new Dictionary<string, ListBox>();

        public ChatControl(TabControl chatControl)
        {
            TabControl = chatControl;
        }

        public void AddTabItem(string tabName)
        {
            if (TabItems.ContainsKey(tabName))
            {
                DeleteTabItem(tabName);
                CreateTabItem(tabName);
            }
            else
                CreateTabItem(tabName);

            TabControl.Items.Add(TabItems[tabName]);
            TabControl.SelectedIndex = TabControl.Items.Count - 1;
        }

        public void DeleteTabItem(string tabName)
        {
            if (TabItems.ContainsKey(tabName))
            {
                TabControl.Items.Remove(TabItems[tabName]);
                TabItems.Remove(tabName);
                MessageButton.Remove(tabName);
                ChatBox.Remove(tabName);
                TextBox.Remove(tabName);
                UsersBox.Remove(tabName);
            }
        }

        public void RenameTabItem(string oldName, string newName)
        {
            foreach (KeyValuePair<string, TabItem> keyValuePair in TabItems)
                if (keyValuePair.Key == oldName)
                {
                    TabItems[keyValuePair.Key].Name = newName;
                    TabItems[keyValuePair.Key].Header = newName;
                    TabItem tabItem = TabItems[keyValuePair.Key];
                    TabItems.Remove(keyValuePair.Key);
                    TabItems.Add(newName, tabItem);

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

        public async void UpdateStatusUserToListBox(string tabName, string userName, string status)
        {
            foreach (KeyValuePair<string, TabItem> keyValuePair in TabItems)
                if (keyValuePair.Key == tabName)
                {
                    ItemCollection ic = UsersBox[keyValuePair.Key].Items;
                    for (int i = 0; i < ic.Count; i++)
                    {
                        if (ic[i].ToString() == userName)
                            switch (status)
                            {
                                case "banned":
                                    ic[i] = userName + $" [{status}]";
                                    break;
                                case "administrator":
                                    ic[i] = userName + $" [{status}]";
                                    break;
                                case "moderator":
                                    ic[i] = userName + $" [{status}]";
                                    break;
                                case "creator":
                                    ic[i] = userName + $" [{status}]";
                                    break;
                                default:
                                    if (ic[i].ToString() == userName + " [banned]")
                                        ic[i] = userName;
                                    break;
                            }
                    }
                    break;
                }
        }

        public void DeleteAllTabItem()
        {
            TabControl.Items.Clear();
            TabItems.Clear();
            MessageButton.Clear();
            ChatBox.Clear();
            TextBox.Clear();
            UsersBox.Clear();
        }

        public static bool HasUserToUsersBox(string tabName, string userName)
        {
            string[] statusWords = { "banned", "creator", "administrator", "moderator" };

            foreach (string statusWord in statusWords)
                for (int i = 0; i < UsersBox[tabName].Items.Count; i++)
                    if (UsersBox[tabName].Items[i].ToString() == userName + $" [{statusWord}]" ||
                        UsersBox[tabName].Items[i].ToString() == userName)
                        return true;

            return false;
        }

        public static bool HasTabItemToDictionary(string tabName)
        {
            if (TabItems.ContainsKey(tabName))
                return true;

            return false;
        }

        private void CreateTabItem(string tabName)
        {
            ChatBox.Add(tabName, new ListBox
            {
                Name = tabName,
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = 272,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 617
            });
            TextBox.Add(tabName, new TextBox
            {
                Name = tabName,
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = 50,
                Margin = new Thickness(0, 277, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Width = 617
            });
            UsersBox.Add(tabName, new ListBox
            {
                Name = tabName,
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = 272,
                Margin = new Thickness(622, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Width = 144
            });
            MessageButton.Add(tabName, new Button
            {
                Name = tabName,
                Content = "Отправить",
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(622, 277, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Width = 75
            });
            MessageButton[tabName].Click += MainWindow.MessageButton_Click;


            Grid chatGrid = new Grid();
            chatGrid.Name = "ChatGrid";
            chatGrid.Background = Brushes.WhiteSmoke;
            chatGrid.Children.Add(ChatBox[tabName]);
            chatGrid.Children.Add(TextBox[tabName]);
            chatGrid.Children.Add(UsersBox[tabName]);
            chatGrid.Children.Add(MessageButton[tabName]);

            TabItems.Add(tabName,
                new TabItem()
                {
                    Name = tabName,
                    Header = tabName,
                    Margin = new Thickness(-2, -2, 2, 0),
                    Content = chatGrid
                });
        }
    }
}