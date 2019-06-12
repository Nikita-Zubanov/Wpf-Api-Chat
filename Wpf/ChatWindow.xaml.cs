using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Логика взаимодействия для CreatingChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        private TabControl ChatControl;
        public static Dictionary<string, Button> MessageButton = new Dictionary<string, Button>();
        public static Dictionary<string, ListBox> ChatBox = new Dictionary<string, ListBox>();
        public static Dictionary<string, TextBox> TextBox = new Dictionary<string, TextBox>();
        public static Dictionary<string, ListBox> UsersBox = new Dictionary<string, ListBox>();

        public ChatWindow(TabControl chatControl)
        {
            InitializeComponent();

            ChatControl = chatControl;
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            string chatName = ChatNameBox.Text;

            if (chatName != string.Empty)
            {
                ChatControl.Items.Add(CreateTabItem(chatName));
                ChatControl.SelectedIndex = ChatControl.Items.Count - 1;

                ApiManager.Create("api/chats", $"{{'Name':'{chatName}', 'Creator':'{User.Name}'}}");
                ApiManager.Create("api/chat/user", $"{{'ChatName':'{chatName}', 'UserName':'{User.Name}'}}");

                Close();
            }
            else
                MessageBox.Show("Заполнит поле!");
        }

        private TabItem CreateTabItem(string tabName)
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
            MessageButton.Add(tabName, new Button {
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

            TabItem item = new TabItem();
            item.Name = tabName;
            item.Header = tabName;
            item.Margin = new Thickness(-2, -2, 2, 0);
            item.Content = chatGrid;

            return item;
        }
    }
}
