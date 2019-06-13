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

        [Route("message")]
        [HttpPost]
        public void AddMessage([FromBody] Chat value)
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
        public ActionResult<IEnumerable<Chat>> GetChat(string name)
        {
            return ChatContext.GetChat(name);
        }
        
        [HttpGet("users/{Name}")]
        public ActionResult<IEnumerable<UsersInChats>> GetUsers(string name)
        {
            return ChatContext.GetUsers(name);
        }

        /*
         *  Для Postman'а
         */
        [HttpGet]
        public ActionResult<IEnumerable<Chat>> Get()
        {
            return ChatContext.GetChat();
        }
    }
}