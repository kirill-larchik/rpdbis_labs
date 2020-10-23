using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    public partial class Show
    {
        public Show()
        {
            Timetables = new HashSet<Timetable>();
        }

        public int ShowId { get; set; }
        public string Name { get; set; }
        public DateTime ReleaseDate { get; set; }
        public TimeSpan Duration { get; set; }
        public int Mark { get; set; }
        public int MarkMonth { get; set; }
        public int MarkYear { get; set; }
        public int GenreId { get; set; }
        public string Description { get; set; }

        public Genre Genre { get; set; }
        public ICollection<Timetable> Timetables { get; set; }

        public override string ToString()
        {
            return $"Название передачи: {Name}, Дата выхода: {ReleaseDate.ToString("d")}, Продолжительность: {Duration};";
        }
    }
}
