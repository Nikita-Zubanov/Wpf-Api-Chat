using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public void Create([FromBody] Chat chat)
        {
            ChatContext.Create(chat);
        }
        
        [HttpDelete("{chatName}/{userName}")]
        public void Delete(string chatName, string userName)
        {
            ChatContext.Delete(chatName, userName);
        }

        [Route("message")]
        [HttpPost]
        public void AddMessage([FromBody] Message message)
        {
            ChatContext.AddMessage(message);
        }

        [Route("user")]
        [HttpPost]
        public void AddUser([FromBody] UserChat userChat)
        {
            ChatContext.AddUser(userChat);
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