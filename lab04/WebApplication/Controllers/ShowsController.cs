using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;

namespace WebApplication.Controllers
{
    public class ShowsController : Controller
    {
        private TvChannelContext db;

        public ShowsController(TvChannelContext context)
        {
            db = context;
        }

        [ResponseCache(CacheProfileName = "CacheProfile")]
        public IActionResult Index()
        {
            return View(db.Shows.Include(s => s.Genre).Take(20).ToList());
        }
    }
}
