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

        #region Chat CRUD-actions
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

        [HttpPut("renameChat/{newChatName}")]
        public void RenameChat([FromBody] Chat chat, string newChatName)
        {
            ChatContext.RenameChat(chat, newChatName);
        }

        [HttpGet("chats/{userName}")]
        public ActionResult<IEnumerable<Chat>> GetChats(string userName)
        {
            return ChatContext.GetChats(userName);
        }
        #endregion

        #region Message-actions
        [Route("message")]
        [HttpPost]
        public async Task AddMessageToChat([FromBody] Message message)
        {
            ChatContext.AddMessageToChat(message);
        }

        [HttpGet("messages/{Name}")]
        public ActionResult<IEnumerable<Message>> GetMessages(string name)
        {
            return ChatContext.GetMessages(name);
        }
        #endregion

        #region User-actions
        [HttpPut("renameUser/{newUserName}")]
        public void RenameUser([FromBody] User user, string newUserName)
        {
            ChatContext.RenameUser(user, newUserName);
        }

        [HttpPut("changeModerator/{isModerator}")]
        public void ChangeModerator([FromBody] User user, bool isModerator)
        {
            ChatContext.ChangeModerator(user, isModerator);
        }

        [HttpPut("banUser/{time}")]
        public void BanUserToChat([FromBody] User user, double time)
        {
            ChatContext.BanUser(user, time);
        }

        [HttpGet("users/{Name}")]
        public ActionResult<IEnumerable<User>> GetUsers(string name)
        {
            return ChatContext.GetUsers(name);
        }

        [HttpGet("statusUser/{chatName}/{userName}")]
        public ActionResult<string> GetStatusUser(string chatName, string userName)
        {
            return ChatContext.GetStatusUser(chatName, userName);
        }
        #endregion

        #region UserChat-actions
        [Route("user")]
        [HttpPost]
        public async Task AddUserToChat([FromBody] UserChat userChat)
        {
            ChatContext.AddUserToChat(userChat);
        }

        [HttpPut("banUserToChat/{time}")]
        public void BanUserToChat([FromBody] UserChat userChat, double time)
        {
            ChatContext.BanUserToChat(userChat, time);
        }
        
        [HttpDelete("removeUserFromChat/{chatName}/{userName}")]
        public void RemoveUserFromChat(string chatName, string userName)
        {
            ChatContext.RemoveUserFromChat(chatName, userName);
        }
        #endregion

        #region Boolean-actions
        [HttpGet("isUserBanned/{chatName}/{userName}")]
        public ActionResult<bool> IsUserBanned(string chatName, string userName)
        {
            return ChatContext.IsUserBanned(chatName, userName);
        }

        [HttpGet("hasLowRightInChat/{chatName}/{userName}")]
        public ActionResult<bool> HasLowRightInChat(string chatName, string userName)
        {
            return ChatContext.HasLowRightInChat(chatName, userName);
        }

        [HttpGet("hasHighRightInChat/{chatName}/{userName}")]
        public ActionResult<bool> HasHighRightInChat(string chatName, string userName)
        {
            return ChatContext.HasHighRightInChat(chatName, userName);
        }

        [HttpGet("isTrueUser/{userName}/{userPassword}")]
        public ActionResult<bool> IsTrueUser(string userName, string userPassword)
        {
            return ChatContext.IsTrueUser(userName, userPassword);
        }

        [HttpGet("isChatExists/{chatName}")]
        public ActionResult<bool> IsChatExists(string chatName)
        {
            return ChatContext.IsChatExists(chatName);
        }

        [HttpGet("isUserExists/{userName}")]
        public ActionResult<bool> IsUserExists(string userName)
        {
            return ChatContext.IsUserExists(userName);
        }
        #endregion
    }
}