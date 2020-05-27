using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ManualAuth.Models;


namespace ManualAuth.Data
{
    public class ApplicationDbContext:IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }


        public DbSet<Pressure> Pressures { get; set; }
        public DbSet<Glucose> Glucoses { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.Seed();

            //builder.Entity<Patient>().HasData(
            //    new Patient() { Id = 1, Email = "pierwszy@gmail.com", Password = "SilneHasło-1", ConfirmPassword = "SilneHasło-1", FirstName = "Paweł", LastName = "Golec", MedDates = new List<MedDate>() },
            //    new Patient() { Id = 2, Email = "drugi@gmail.com", Password = "SilneHasło-2", ConfirmPassword = "SilneHasło-2", FirstName = "Grzegorz", LastName = "Turnau", MedDates = new List<MedDate>() },
        }


        public DbSet<ManualAuth.Models.Medicine> Medicine { get; set; }


        public DbSet<ManualAuth.Models.Pressure> Pressure { get; set; }

    }
}
