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
    public class AuthorizationController : ControllerBase
    {
        private static readonly ChatAppContext UsersContext = new ChatAppContext();

        [Route("register")]
        [HttpPost]
        public void Register([FromBody] User user)
        {
            UsersContext.Register(user, "user", "Online");
        }

        [Route("login")]
        [HttpPut]
        public void Login([FromBody] User user)
        {
            UsersContext.LoginOrLogout(user, "Online");
        }

        [Route("logout")]
        [HttpPut]
        public void Logout([FromBody] User user)
        {
            UsersContext.LoginOrLogout(user, "Offline");
        }
        
        [HttpGet("isRegistred/{Name}/{Password}")]
        public ActionResult<bool> IsRegistred(string name, string password)
        {
            return UsersContext.IsRegistred(name, password);
        }
    }
}