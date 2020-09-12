using MeetUp.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetUp
{
    public class MeetupSeeder
    {
        private readonly MeetupContext _meetupContext;

        public MeetupSeeder(MeetupContext meetupContext)
        {
            _meetupContext = meetupContext;
        }

        public void Seed()
        {
            if (_meetupContext.Database.CanConnect())
            {
                if (!_meetupContext.Meetups.Any())
                {
                    InsertSampleData();
                }
            }
        }

        private void InsertSampleData()
        {
            var meetups = new List<Meetup>
            {
                new Meetup
                {
                    Name = "First meet up",
                    Organizer = "Alice",
                    Date = DateTime.Now.AddDays(7),
                    IsPrivate = false,
                    Location = new Location
                    {
                        City = "Auckland",
                        Street = "Mount Street",
                        PostCode = "1010",
                    },
                    Lectures = new List<Lecture>
                    {
                        new Lecture
                        {
                            Author = "Tom",
                            Topic = "Art",
                            Description = "A lecture on art"
                        },
                        new Lecture
                        {
                            Author = "Jerry",
                            Topic = "Math",
                            Description = "A lecture on math"
                        }
                    }
                },
                new Meetup
                {
                    Name = "Second meet up",
                    Organizer = "Bob",
                    Date = DateTime.Now.AddDays(14),
                    IsPrivate = true,
                    Location = new Location
                    {
                        City = "Wellington",
                        Street = "Sussex Street",
                        PostCode = "6100",
                    },
                    Lectures = new List<Lecture>
                    {
                        new Lecture
                        {
                            Author = "Wang",
                            Topic = "Art",
                            Description = "A lecture on art"
                        },
                        new Lecture
                        {
                            Author = "Zhang",
                            Topic = "Math",
                            Description = "A lecture on math"
                        }
                    }
                },
            };
            _meetupContext.AddRange(meetups);
            _meetupContext.SaveChanges();
        }
    }
}
