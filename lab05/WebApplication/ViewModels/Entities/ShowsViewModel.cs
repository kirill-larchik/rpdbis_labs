using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.ViewModels.Entities
{
    public class ShowsViewModel : IEntitiesViewModel<Show>
    {
        [Display(Name = "Shows")]
        public IEnumerable<Show> Entities { get; set; }
        [Display(Name = "Show")]
        public Show Entity { get; set; }
        [Display(Name = "Genres")]
        public SelectList SelectList { get; set; }


        public PageViewModel PageViewModel { get; set; }
        public DeleteViewModel DeleteViewModel { get; set; }
     
    }
}
