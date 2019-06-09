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
    public partial class CreatingChatWindow : Window
    {
        public static TabControl ChatControl;
        public static Button MessageButton;
        public static ListBox ChatBox;
        public static TextBox TextBox;
        public static ListBox UsersBox;

        public CreatingChatWindow(TabControl chatControl)
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

                ApiManager.Create("api/chats", $"{{'Name':'{chatName}', 'Creator':'{User.Name}'}}");

                Close();
            }
            else
                MessageBox.Show("Заполнит поле!");
        }

        private TabItem CreateTabItem(string tabName)
        {
            ChatBox = new ListBox
            {
                Name = "ChatBox",
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = 272,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 617
            };
            TextBox = new TextBox
            {
                Name = "TextBox",
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = 64,
                Margin = new Thickness(0, 277, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Width = 617
            };
            UsersBox = new ListBox
            {
                Name = "UsersBox",
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = 272,
                Margin = new Thickness(622, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Width = 144
            };
            MessageButton = new Button {
                Name = "MessageButton",
                Content = "Отправить",
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(622, 277, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Width = 75
            };
            MessageButton.Click += MainWindow.MessageButton_Click;

            Grid chatGrid = new Grid();
            chatGrid.Name = "ChatGrid";
            chatGrid.Background = Brushes.WhiteSmoke;
            chatGrid.Children.Add(ChatBox);
            chatGrid.Children.Add(TextBox);
            chatGrid.Children.Add(UsersBox);
            chatGrid.Children.Add(MessageButton);

            TabItem item = new TabItem();
            item.Name = tabName;
            item.Header = tabName;
            item.Margin = new Thickness(-2, -2, 2, 0);
            item.Content = chatGrid;

            return item;
        }
    }
}
