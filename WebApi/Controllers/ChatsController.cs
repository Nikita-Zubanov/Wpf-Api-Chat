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
    public class ChatsController : ControllerBase
    {
        private readonly ChatsContext Chatsstack;

        public ChatsController()
        {
            Chatsstack = ChatsContext.GetChatsStack();
        }

        [HttpPost]
        public void Create([FromBody] Chats chat)
        {
            Chatsstack.Create(chat);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Chats>> Get()
        {
            return Chatsstack.Get();
        }
    }
}