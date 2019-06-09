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
    public class AuthorizationController : ControllerBase
    {
        private readonly UsersContext Userstack;

        public AuthorizationController()
        {
            Userstack = UsersContext.GetUsersStack();
        }

        [HttpPost]
        public void Login([FromBody] User user)
        {
            Userstack.Post(user);
        }

        [HttpPut("{Name}")]
        public void Logout(string name, [FromBody] User user)
        {
            Userstack.Put(name, user);
        }
    }
}