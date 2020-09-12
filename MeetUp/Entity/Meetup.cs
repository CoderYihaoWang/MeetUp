using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetUp.Entity
{
    public class Meetup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Organizer { get; set; }
        public DateTime Date { get; set; }
        public bool IsPrivate { get; set; }

        // virtual is for lazy loading, which uses proxy
        public virtual Location Location { get; set; }
        public virtual List<Lecture> Lectures { get; set; }
    }
}
