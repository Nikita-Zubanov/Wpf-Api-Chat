using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public static void AddUserToUserBox(string tabName, string userName)
        {
            UsersBox[tabName].Items.Add(userName);
        }

        public void CreateTabItem(string tabName)
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

            //return item;
        }
    }
}

