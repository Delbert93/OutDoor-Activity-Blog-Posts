using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HW_6.Models;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using HW_6.Data;
using Microsoft.EntityFrameworkCore;

namespace HW_6.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController()
        {

        }

        // GET: BlogPosts
        public async Task<IActionResult> IndexPosts()
        {
            return View(await _context.BlogPosts.ToListAsync());
        }

        public async Task<IActionResult> Index(string permalink)
        {
            //implementing the CMS Code
            ViewData["permalink"] = permalink;
            return View();  
        }

        public IActionResult Rubric()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
