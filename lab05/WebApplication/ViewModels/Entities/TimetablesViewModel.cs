using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.ViewModels.Entities
{
    public class TimetablesViewModel : IEntitiesViewModel<Timetable>
    {
        [Display(Name = "Timetables")]
        public IEnumerable<Timetable> Entities { get; set; }
        [Display(Name = "Timetable")]
        public Timetable Entity { get; set; }
        public SelectList SelectList { get; set; }

        public PageViewModel PageViewModel { get; set; }
        public DeleteViewModel DeleteViewModel { get; set; }
       
    }
}
