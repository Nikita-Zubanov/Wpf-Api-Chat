using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Hubs
{
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string chatName, string userName, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", chatName, userName, message);
        }

        public async Task AddUserToChat(string chatName, string userName, string status)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatName);
            await Clients.OthersInGroup(chatName).SendAsync("ReceiveUser", chatName, userName, status);
        }

        public async Task UpdateUser(string oldName, string newName)
        {
            await Clients.All.SendAsync("ChangeUser", oldName, newName);
        }

        public async Task UpdateChat(string oldName, string newName)
        {
            await Clients.All.SendAsync("ChangeChat", oldName, newName);
        }

        public async Task RemoveUserFromChat(string chatName, string userName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatName);
            await Clients.OthersInGroup(chatName).SendAsync("RemoveUser");
        }

        public async Task RemoveChat(string chatName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatName);
            await Clients.Group(chatName).SendAsync("RemoveChat", chatName);
        }

        public async Task BanUserToChat(string chatName, string userName)
        {
            await Clients.All.SendAsync("BanUser", chatName, userName);
        }
    }
}
