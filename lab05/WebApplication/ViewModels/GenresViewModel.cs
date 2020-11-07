using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.ViewModels
{
    public class GenresViewModel
    {
        public IEnumerable<Genre> Genres { get; set; }
        public Genre Genre { get; set; }


        [Display(Name = "Modification")]
        public string Modification { get; set; }
       

        public PageViewModel PageViewModel { get; set; }
        public int CurrentHomePage { get; set; }


        public DeleteViewModel DeleteViewModel { get; set; }
    }
}
