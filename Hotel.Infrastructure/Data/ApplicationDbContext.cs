﻿using Hotel.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Villa> Villas { get; set; }
        public DbSet<VillaNumber> VillaNumbers { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Villa>().HasData(
                new Villa
                {
                    Id = 1,
                    Name = "Royal Villa",
                    Description = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    ImageUrl = "https://placehold.co/600x400",
                    Occupancy = 4,
                    Price = 200,
                    Square = 550,
                },
                new Villa
                {
                    Id = 2,
                    Name = "Premium Pool Villa",
                    Description = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    ImageUrl = "https://placehold.co/600x401",
                    Occupancy = 4,
                    Price = 300,
                    Square = 550,
                },
                new Villa
                {
                    Id = 3,
                    Name = "Luxury Pool Villa",
                    Description = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    ImageUrl = "https://placehold.co/600x402",
                    Occupancy = 4,
                    Price = 400,
                    Square = 750,
                }, new Villa
                {
                    Id = 4,
                    Name = "Aveno",
                    Description = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    ImageUrl = "https://placehold.co/600x402",
                    Occupancy = 5,
                    Price = 100,
                    Square = 800,
                });
            modelBuilder.Entity<VillaNumber>().HasData(
                new VillaNumber
                {
                    Villa_Number = 101,
                    VillaId = 1
                },
                new VillaNumber
                {
                    Villa_Number = 102,
                    VillaId = 1
                },
                new VillaNumber
                {
                    Villa_Number = 103,
                    VillaId = 1
                },
                new VillaNumber
                {
                    Villa_Number = 201,
                    VillaId = 2,
                },
                new VillaNumber
                {
                    Villa_Number = 202,
                    VillaId = 2
                },
                new VillaNumber
                {
                    Villa_Number = 203,
                    VillaId = 2
                },
                new VillaNumber
                {
                    Villa_Number = 301,
                    VillaId = 3
                },
                new VillaNumber
                {
                    Villa_Number = 302,
                    VillaId = 3
                },
                new VillaNumber
                {
                    Villa_Number = 303,
                    VillaId = 2
                },
                new VillaNumber
                {
                    Villa_Number = 401,
                    VillaId = 4
                },
                new VillaNumber
                {
                    Villa_Number = 402,
                    VillaId = 4
                });
            modelBuilder.Entity<Amenity>().HasData(
                new Amenity
                {
                    Id = 1,
                    VillaId = 1,
                    Name = "Private Pool"
                }, new Amenity
                {
                    Id = 2,
                    VillaId = 1,
                    Name = "Microwave"
                }, new Amenity
                {
                    Id = 3,
                    VillaId = 1,
                    Name = "Private Balcony"
                }, new Amenity
                {
                    Id = 4,
                    VillaId = 1,
                    Name = "1 king bed and 1 sofa bed"
                },

                new Amenity
                {
                    Id = 5,
                    VillaId = 2,
                    Name = "Private Plunge Pool"
                }, new Amenity
                {
                    Id = 6,
                    VillaId = 2,
                    Name = "Microwave and Mini Refrigerator"
                }, new Amenity
                {
                    Id = 7,
                    VillaId = 2,
                    Name = "Private Balcony"
                }, new Amenity
                {
                    Id = 8,
                    VillaId = 2,
                    Name = "king bed or 2 double beds"
                },

                new Amenity
                {
                    Id = 9,
                    VillaId = 3,
                    Name = "Private Pool"
                }, new Amenity
                {
                    Id = 10,
                    VillaId = 3,
                    Name = "Jacuzzi"
                }, new Amenity
                {
                    Id = 11,
                    VillaId = 3,
                    Name = "Private Balcony"
                });

        }
    }
}
