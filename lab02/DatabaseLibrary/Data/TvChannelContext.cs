using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using DatabaseLibrary.Models;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DatabaseLibrary.Data
{
    public partial class TvChannelContext : DbContext
    {
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<Show> Shows { get; set; }
        public virtual DbSet<Timetable> Timetables { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("config.json");

            IConfigurationRoot configuration = builder.Build();
            string connectionString = configuration.GetConnectionString("SQLConnection");

            DbContextOptions options = optionsBuilder
                .UseSqlServer(connectionString)
                .Options;
        }
    }
}
