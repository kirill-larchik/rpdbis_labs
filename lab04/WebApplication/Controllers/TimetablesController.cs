using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;

namespace WebApplication.Controllers
{
    public class TimetablesController : Controller
    {
        private TvChannelContext db;

        public TimetablesController(TvChannelContext context)
        {
            db = context;
        }

        [ResponseCache(CacheProfileName = "CacheProfile")]
        public IActionResult Index()
        {
            return View(db.Timetables.Include(t => t.Show).Take(20).ToList());
        }
    }
}
