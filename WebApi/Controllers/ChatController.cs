using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApi.Hubs;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private static readonly ChatAppContext ChatContext = new ChatAppContext();

        [Route("create")]
        [HttpPost]
        public void CreateChat([FromBody] Chat chat)
        {
            ChatContext.Create(chat);
        }
        
        [HttpDelete("deleteChat/{chatName}/{userName}")]
        public void DeleteChat(string chatName, string userName)
        {
            ChatContext.Delete(chatName, userName);
        }

        [HttpDelete("removeUserFromChat/{chatName}/{userName}/{disabledUser}/{time}")]
        public void BanUserFromChat(string chatName, string userName, string disabledUser, double time)
        {
            ChatContext.BanUserToChat(chatName, userName, disabledUser, time);
        }

        [HttpDelete("removeUserFromChat/{chatName}/{userName}")]
        public void RemoveUserFromChat(string chatName, string userName)
        {
            ChatContext.RemoveUserFromChat(chatName, userName);
        }

        [Route("message")]
        [HttpPost]
        public async Task AddMessageToChat([FromBody] Message message)
        {
            ChatContext.AddMessageToChat(message);
        }

        [Route("user")]
        [HttpPost]
        public async Task AddUserToChat([FromBody] UserChat userChat)
        {
            ChatContext.AddUserToChat(userChat);
        }

        [HttpGet("messages/{Name}")]
        public ActionResult<IEnumerable<Message>> GetMessages(string name)
        {
            return ChatContext.GetMessages(name);
        }
        
        [HttpGet("users/{Name}")]
        public ActionResult<IEnumerable<User>> GetUsers(string name)
        {
            return ChatContext.GetUsers(name);
        }

        [HttpGet("{chatName}/{userName}")]
        public ActionResult<bool> IsUserBanned(string chatName, string userName)
        {
            return ChatContext.IsUserBanned(chatName, userName);
        }

        #region Methods for "postman"
        [Route("chats")]
        [HttpGet]
        public ActionResult<IEnumerable<Chat>> GetChats()
        {
            return ChatContext.GetChats();
        }
        #endregion
    }
}