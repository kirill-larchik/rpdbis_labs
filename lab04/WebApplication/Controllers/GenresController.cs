using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Data;

namespace WebApplication.Controllers
{
    public class GenresController : Controller
    {
        private TvChannelContext db;

        public GenresController(TvChannelContext context)
        {
            db = context;
        }

        [ResponseCache(CacheProfileName = "CacheProfile")]
        public IActionResult Index()
        {
            return View(db.Genres.Take(20).ToList());
        }
    }
}
