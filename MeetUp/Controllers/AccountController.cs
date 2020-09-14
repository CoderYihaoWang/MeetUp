using AutoMapper;
using MeetUp.Entity;
using MeetUp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetUp.Controllers
{
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly MeetupContext _meetupContext;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountController(MeetupContext meetupContext, IPasswordHasher<User> passwordHasher)
        {
            _meetupContext = meetupContext;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("register")]
        public ActionResult Register([FromBody] RegisterUserDto registerUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new User
            {
                Email = registerUserDto.Email,
                Nationality = registerUserDto.Nationality,
                DateOfBirth = registerUserDto.DateOfBirth,
                RoleId = registerUserDto.RoleId
            };
            var passwordHash = _passwordHasher.HashPassword(user, registerUserDto.Password);
            user.PasswordHash = passwordHash;
            _meetupContext.Users.Add(user);
            _meetupContext.SaveChanges();
            return Ok();
        }
    }
}
