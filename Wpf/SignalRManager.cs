using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf
{
    class SignalRManager : MainWindow
    {
        public static HubConnection connection;
        public static MainWindow mainWindow;

        public const string urlSignalR = "http://localhost:58269/";
        public const string uriSignalR = "ChatHub";
        
        public async void OnConnect()
        {
            connection.On<string, string, string>("ReceiveMessage", (chatName, userName, message) =>
            {
                Dispatcher.Invoke(() =>
                {
                    string newMessage = $"{userName}: {message}";
                    if (ChatControl.ChatBox.Count != 0)
                        if (ChatControl.HasTabItemToDictionary(chatName))
                            ChatControl.ChatBox[chatName].Items.Add(newMessage);
                });
            });
            connection.On<string, string>("ReceiveUser", (chatName, userName) =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (ChatControl.UsersBox.Count != 0)
                        if (!ChatControl.HasUserToUsersBox(chatName, userName))
                            ChatControl.UsersBox[chatName].Items.Add(userName);
                });
            });
            connection.On<string, string>("ChangeUser", (oldName, newName) =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (User.Name == oldName)
                        User.Name = newName;

                    UpdateUsersBox();
                });
            });
            connection.On<string, string>("BanUser", (chatName, userName) =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (User.Name == userName)
                    {
                        ChatControl chatWindow = new ChatControl(ChatsControl);
                        chatWindow.DeleteTabItem(chatName);
                    }
                });
            });

            await connection.StartAsync();
        }
        public async Task OnDisconnect()
        {
            await connection.StopAsync();
            connection = null;
        }

        public async void SendMessage(string chatName, string userName, string message)
        {
            await connection.InvokeAsync("SendMessage", chatName, userName, message);
        }

        public async void AddUserToChat(string chatName, string userName)
        {
            await connection.InvokeAsync("AddUserToChat", chatName, userName);
        }

        public async void UpdateUser(string oldName, string newName)
        {
            await connection.InvokeAsync("UpdateUser", oldName, newName);
        }

        public async void RemoveUserFromChat(string chatName, string userName)
        {
            await connection.InvokeAsync("RemoveUserFromChat", chatName, userName);
        }

        public async void BanUserToChat(string chatName, string userName)
        {
            await connection.InvokeAsync("BanUserToChat", chatName, userName);
        }
    }
}