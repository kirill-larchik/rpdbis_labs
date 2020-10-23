using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private TvChannelContext db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, TvChannelContext context)
        {
            _logger = logger;
            db = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(CacheProfileName = "CacheProfile")]
        public IActionResult Genres()
        {
            return View(db.Genres.Take(20).ToList());
        }

        [ResponseCache(CacheProfileName = "CacheProfile")]
        public IActionResult Shows()
        {
            return View(db.Shows.Include(s => s.Genre).Take(20).ToList());
        }

        [ResponseCache(CacheProfileName = "CacheProfile")]
        public IActionResult Timetables()
        {
            return View(db.Timetables.Include(t => t.Show).Take(20).ToList());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
