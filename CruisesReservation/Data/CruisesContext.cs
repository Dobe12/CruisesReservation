using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CruisesReservation.Models;
using Microsoft.EntityFrameworkCore;

namespace CruisesReservation.Data
{
    public sealed class CruisesContext : DbContext
    {
        public DbSet<Cruises> Cinemas { get; set; }
        public DbSet<Ship> Ships { get; set; }
        public DbSet<Seat> Seats { get; set; }

        public CruisesContext(DbContextOptions<CruisesContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cruises>().HasData(new List<Cruises>
            {
                new Cruises
                {
                    Id = 1,
                    Name = "Carnival",
                    Country = "Andorra",
                }
            });
            modelBuilder.Entity<Ship>().HasData(new List<Ship>
            {
                new Ship
                {
                    Id = 1,
                    Number = 8,
                    CruisesId = 1 
                }
            });
            modelBuilder.Entity<Seat>().HasData(new List<Seat>
            {
                new Seat
                {
                    Id = 1,
                    IsReserved = false,
                    Place = 10,
                    Row = 5,
                    ShipId = 1
                },
                new Seat
                {
                    Id = 2,
                    IsReserved = true,
                    Place = 10,
                    Row = 5,
                    ShipId = 1,
                    PlaceHolderPhone = 89345678665
                },

            });
        }
    }
}
