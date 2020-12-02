using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.ViewModels
{
    public class ShowViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Duration { get; set; }
        public int Mark { get; set; }
        public int MarkMonth { get; set; }
        public int MarkYear { get; set; }
        public int GenreId { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
    }
}
