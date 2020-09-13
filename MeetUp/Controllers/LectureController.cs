﻿using AutoMapper;
using MeetUp.Entity;
using MeetUp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetUp.Controllers
{
    [Route("api/meetup/{meetupName}/lecture")]
    public class LectureController : ControllerBase
    {
        private readonly MeetupContext _meetupContext;
        private readonly IMapper _mapper;
        private readonly ILogger<LectureController> _logger;

        public LectureController(MeetupContext meetupContext, IMapper mapper, ILogger<LectureController> logger)
        {
            _meetupContext = meetupContext;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<List<LectureDto>> Get(string meetupName)
        {
            var meetup = _meetupContext.Meetups
                .Include(m => m.Lectures)
                .FirstOrDefault(m => m.Name.Replace(" ", "-").ToLower() == meetupName.ToLower());
            if (meetup is null)
            {
                return NotFound();
            }
            var lectures = meetup.Lectures;
            var lectureDtos = _mapper.Map<List<LectureDto>>(lectures);
            return Ok(lectureDtos);
        }

        [HttpPost]
        public ActionResult Post(string meetupName, [FromBody] LectureDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var meetup = _meetupContext.Meetups
                .Include(m => m.Lectures)
                .FirstOrDefault(m => m.Name.Replace(" ", "-").ToLower() == meetupName.ToLower());
            if (meetup is null)
            {
                return NotFound();
            }
            var lecture = _mapper.Map<Lecture>(model);
            meetup.Lectures.Add(lecture);
            _meetupContext.SaveChanges();
            return Created($"api/meetup/{meetupName}", null);
        }

        [HttpDelete]
        public ActionResult Delete(string meetupName)
        {
            var meetup = _meetupContext.Meetups
                .Include(m => m.Lectures)
                .FirstOrDefault(m => m.Name.Replace(" ", "-").ToLower() == meetupName.ToLower());
            if (meetup is null)
            {
                return NotFound();
            }

            _logger.LogWarning($"Lectures for meetup {meetupName} were deleted");

            _meetupContext.Lectures.RemoveRange(meetup.Lectures);
            _meetupContext.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(string meetupName, int id)
        {
            var meetup = _meetupContext.Meetups
                .Include(m => m.Lectures)
                .FirstOrDefault(m => m.Name.Replace(" ", "-").ToLower() == meetupName.ToLower());
            if (meetup is null)
            {
                return NotFound();
            }
            var lecture = meetup.Lectures.FirstOrDefault(l => l.Id == id);
            if (lecture is null)
            {
                return NotFound();
            }
            _meetupContext.Lectures.Remove(lecture);
            _meetupContext.SaveChanges();
            return NoContent();
        }
    }
}
