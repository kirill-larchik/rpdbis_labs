using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public partial class Genre
    {
        public Genre()
        {
            Shows = new HashSet<Show>();
        }

        public int GenreId { get; set; }
        public string GenreName { get; set; }
        public string GenreDescription { get; set; }

        public ICollection<Show> Shows { get; set; }

        public override string ToString()
        {
            return $"Название жанра: {GenreName}, Описание {GenreDescription};";
        }
    }
}
