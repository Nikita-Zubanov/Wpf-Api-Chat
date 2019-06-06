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
    public class UsersController : ControllerBase
    {
        private readonly UsersContext Userstack;

        public UsersController()
        {
            Userstack = UsersContext.GetUsersStack();
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> Get()
        {
            return Userstack.Get();
        }

        [HttpGet("{id}")]
        public ActionResult<User> Get(int id)
        {
            return Userstack.Get(id);
        }

        [HttpPost]
        public void Post([FromBody] User value)
        {
            Userstack.Post(value);
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] User value)
        {
            Userstack.Put(id, value);
        }

        [HttpDelete("{Name}")]
        public void Delete(string name)
        {
            Userstack.Delete(name);
        }
    }
}