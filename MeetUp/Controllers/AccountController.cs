using AutoMapper;
using MeetUp.Entity;
using MeetUp.Identity;
using MeetUp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IJwtProvider _jwtProvider;
        public AccountController(MeetupContext meetupContext, IPasswordHasher<User> passwordHasher, IJwtProvider jwtProvider)
        {
            _meetupContext = meetupContext;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
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

        [HttpPost("login")]
        public ActionResult Login([FromBody] UserLoginDto model)
        {
            var user = _meetupContext.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == model.Email);
            if (user is null)
            {
                return BadRequest("Invalid username or password");
            }
            var passwordVerificationResult = _passwordHasher
                .VerifyHashedPassword(user, user.PasswordHash, model.Password);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return BadRequest("Invalid username or password");
            }
            var token = _jwtProvider.GenerateJwtToken(user);
            return Ok(token);
        }
    }
}
