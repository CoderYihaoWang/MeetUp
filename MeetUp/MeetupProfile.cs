using AutoMapper;
using MeetUp.Entity;
using MeetUp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetUp
{
    public class MeetupProfile : Profile
    {
        public MeetupProfile()
        {
            CreateMap<Meetup, MeetupDetailsDto>()
                .ForMember(m => m.City, map => map.MapFrom(meetup => meetup.Location.City))
                .ForMember(m => m.Street, map => map.MapFrom(meetup => meetup.Location.Street))
                .ForMember(m => m.PostCode, map => map.MapFrom(meetup => meetup.Location.PostCode));

            CreateMap<MeetupDto, Meetup>();

            CreateMap<Lecture, LectureDto>()
                .ReverseMap();
        }
    }
}
