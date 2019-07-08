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

        public async Task AddUserToChat(string chatName, string userName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatName);
            await Clients.OthersInGroup(chatName).SendAsync("ReceiveUser", chatName, userName);
        }

        public async Task RenameUser(string oldName, string newName)
        {
            await Clients.All.SendAsync("RenameUser", oldName, newName);
        }

        public async Task ChangeRoleUser()
        {
            await Clients.All.SendAsync("ChangeRoleUser");
        }

        public async Task RenameChat(string oldName, string newName)
        {
            await Clients.All.SendAsync("RenameChat", oldName, newName);
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
