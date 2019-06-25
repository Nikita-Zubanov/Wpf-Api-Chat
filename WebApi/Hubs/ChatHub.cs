using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task AddUserToChat(string chatName, string userName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatName);
            await Clients.OthersInGroup(chatName).SendAsync("ReceiveUser", chatName, userName);
        }

        public async Task RemoveUserFromChat(string chatName, string userName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatName);
            await Clients.OthersInGroup(chatName).SendAsync("RemoveUser", chatName, userName);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }



        public async Task BanUserToChat(string chatName, string userName)
        {
            await Clients.All.SendAsync("BanUser", chatName, userName);
        }
    }
}
