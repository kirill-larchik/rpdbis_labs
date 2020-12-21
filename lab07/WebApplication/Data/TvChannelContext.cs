using System;
using WebApplication.Models;
using System.IO;
using System.Data.Entity;

namespace WebApplication.Data
{
    public partial class TvChannelContext : DbContext
    {
        public TvChannelContext() : base("name=SqlConnection") { }

        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<Show> Shows { get; set; }
        public virtual DbSet<Timetable> Timetables { get; set; }
    }
}
