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

        [Route("message")]
        [HttpPost]
        public void AddMessage([FromBody] Message value)
        {
            ChatContext.AddMessage(value);
        }

        [Route("user")]
        [HttpPost]
        public void AddUser([FromBody] UserChat userChat)
        {
            ChatContext.AddUser(userChat);
        }

        [HttpGet("{Name}")]
        public ActionResult<IEnumerable<Message>> GetChat(string name)
        {
            return ChatContext.GetChat(name);
        }
        
        [HttpGet("users/{Name}")]
        public ActionResult<IEnumerable<User>> GetUsers(string name)
        {
            return ChatContext.GetUsers(name);
        }

        /*
         *  Для Postman'а
         */
        [Route("getMessages")]
        [HttpGet]
        public ActionResult<IEnumerable<Message>> GetMessages()
        {
            return ChatContext.GetMessages();
        }

        [Route("getChats")]
        [HttpGet]
        public ActionResult<IEnumerable<Chat>> GetChats()
        {
            return ChatContext.GetChats();
        }
    }
}