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
        private static readonly ChatsContext ChatsContext = new ChatsContext();

        [HttpPost]
        public void Create([FromBody] Chats chat)
        {
            ChatsContext.Create(chat);
        }

        /*
         *  Для Postman'а
         */
        [HttpGet]
        public ActionResult<IEnumerable<Chats>> Get()
        {
            return ChatsContext.Get();
        }
    }
}