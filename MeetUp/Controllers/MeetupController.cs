﻿using AutoMapper;
using MeetUp.Authorization;
using MeetUp.Entity;
using MeetUp.Filters;
using MeetUp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MeetUp.Controllers
{
    [Route("api/meetup")]
    [Authorize]
    [ServiceFilter(typeof(TimeTrackFilter))]
    public class MeetupController : ControllerBase
    {
        private readonly MeetupContext _meetupContext;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;

        public MeetupController(MeetupContext meetupContext, IMapper mapper, IAuthorizationService authorizationService)
        {
            _meetupContext = meetupContext;
            _mapper = mapper;
            _authorizationService = authorizationService;
        }
        
        [HttpGet]
        [NationalityFilter("German,Russian")]
        public ActionResult<List<MeetupDetailsDto>> Get()
        {
            var meetups = _meetupContext.Meetups.Include(m => m.Location).ToList();
            var meetupDtos = _mapper.Map<List<MeetupDetailsDto>>(meetups);
            return Ok(meetupDtos);
        }

        [HttpGet("{name}")]
        [Authorize(Policy = "AtLeast18")]
        [NationalityFilter("English")]
        public ActionResult<MeetupDetailsDto> Get(string name)
        {
            var meetup = _meetupContext.Meetups
                .Include(m => m.Location)
                .Include(m => m.Lectures)
                .FirstOrDefault(m => m.Name.Replace(" ", "-").ToLower() == name.ToLower());
            if (meetup is null)
            {
                return NotFound();
            }
            var meetupDto = _mapper.Map<MeetupDetailsDto>(meetup);
            return Ok(meetupDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult Post([FromBody] MeetupDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var meetup = _mapper.Map<Meetup>(model);

            var userId = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;

            meetup.CreatedById = int.Parse(userId);

            _meetupContext.Meetups.Add(meetup);
            _meetupContext.SaveChanges();
            var key = meetup.Name.Replace(" ", "-").ToLower();
            return Created($"/api/meetup/{key}", null);
        }

        [HttpPut("{name}")]
        public ActionResult Put(string name, [FromBody] MeetupDto model)
        {
            var meetup = _meetupContext.Meetups
                .FirstOrDefault(m => m.Name.Replace(" ", "-").ToLower() == name.ToLower());
            if (meetup is null)
            {
                return NotFound();
            }

            var authorizationResult = _authorizationService
                .AuthorizeAsync(User, meetup, new ResourceOperationRequirement(OperationType.Update))
                .Result;

            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            meetup.Name = model.Name;
            meetup.Organizer = model.Organizer;
            meetup.Date = model.Date;
            meetup.IsPrivate = model.IsPrivate;
            _meetupContext.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{name}")]
        public ActionResult Delete(string name)
        {
            var meetup = _meetupContext.Meetups
                .FirstOrDefault(m => m.Name.Replace(" ", "-").ToLower() == name.ToLower());
            if (meetup is null)
            {
                return NotFound();
            }

            var authorizationResult = _authorizationService
               .AuthorizeAsync(User, meetup, new ResourceOperationRequirement(OperationType.Delete))
               .Result;

            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            _meetupContext.Meetups.Remove(meetup);
            _meetupContext.SaveChanges();
            return NoContent();
        }
    }
}
