﻿using System;
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

        [HttpGet("users/{Name}")]
        public ActionResult<IEnumerable<User>> GetUsers(string name)
        {
            return ChatContext.GetUsers(name);
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

        [HttpGet("hasRightToChat/{chatName}/{userName}")]
        public ActionResult<bool> HasRightToChat(string chatName, string userName)
        {
            return ChatContext.HasRightToChat(chatName, userName);
        }

        [HttpGet("isUserHasRight/{userName}/{userPassword}")]
        public ActionResult<bool> IsUserHasRight(string userName, string userPassword)
        {
            return ChatContext.IsUserHasRight(userName, userPassword);
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