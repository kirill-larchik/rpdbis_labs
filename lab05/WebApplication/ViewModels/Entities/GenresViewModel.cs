﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.ViewModels.Entities
{
    public class GenresViewModel : IEntitiesViewModel<Genre>
    {
        [Display(Name = "Genres")]
        public IEnumerable<Genre> Entities { get; set; }
        [Display(Name = "Genre")]
        public Genre Entity { get; set; }


        public PageViewModel PageViewModel { get; set; }
        public DeleteViewModel DeleteViewModel { get; set; }
       
    }
}
