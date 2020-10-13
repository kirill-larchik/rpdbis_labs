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
        public TvChannelContext(DbContextOptions<TvChannelContext> options)
            : base(options) { }
        
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<Show> Shows { get; set; }
        public virtual DbSet<Timetable> Timetables { get; set; }
    }
}
