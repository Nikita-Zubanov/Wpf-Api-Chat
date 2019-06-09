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
        private readonly ChatContext Chatstack;

        public ChatController()
        {
            Chatstack = ChatContext.GetChatStack();
        }

        [HttpPost("{message}")]
        public void AddMessage([FromBody] Chat value)
        {
            Chatstack.AddMessage(value);
        }

        [HttpGet("{Name}")]
        public ActionResult<IEnumerable<Chat>> GetCurrentChat(string name)
        {
            return Chatstack.GetCurrentChat(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>


        [HttpGet]
        public ActionResult<IEnumerable<Chat>> Get()
        {
            return Chatstack.Get();
        }

        //[HttpGet("{author}")]
        //public ActionResult<Chat> Get(string author)
        //{
        //    return Chatstack.Get(author);
        //}

        [HttpPost]
        public void Post([FromBody] Chat value)
        {
            Chatstack.Post(value);
        }

        [HttpPut("{author}")]
        public void Put(string author, [FromBody] Chat value)
        {
            Chatstack.Put(author, value);
        }

        [HttpDelete("{author}")]
        public void Delete(string author)
        {
            Chatstack.Delete(author);
        }
    }
}