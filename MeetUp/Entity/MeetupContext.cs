using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetUp.Entity
{
    public class MeetupContext : DbContext
    {
        private readonly string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public DbSet<Meetup> Meetups { get; set; }
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Meetup>()
                .HasOne(m => m.Location)
                .WithOne(l => l.Meetup)
                .HasForeignKey<Location>(l => l.MeetupId);

            modelBuilder.Entity<Meetup>()
                .HasMany(m => m.Lectures)
                .WithOne(l => l.Meetup);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
