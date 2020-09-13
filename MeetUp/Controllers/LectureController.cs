using AutoMapper;
using MeetUp.Entity;
using MeetUp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public LectureController(MeetupContext meetupContext, IMapper mapper)
        {
            _meetupContext = meetupContext;
            _mapper = mapper;
        }

        public ActionResult<List<LectureDto>> Get(string meetupName)
        {
            var lectures = _meetupContext.Meetups
                .Include(m => m.Lectures)
                .FirstOrDefault(m => m.Name.Replace(" ", "-").ToLower() == meetupName.ToLower())
                ?.Lectures;
            var lectureDtos = _mapper.Map<List<LectureDto>>(lectures);
            return Ok(lectureDtos);
        }
    }
}
