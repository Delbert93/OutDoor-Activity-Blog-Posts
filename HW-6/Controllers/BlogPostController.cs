using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HW_6;
using HW_6.Data;
using HW_6.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using static HW_6.Models.BlogPost;

namespace HW_6.Controllers
{
    public class BlogPostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly string uploadsPath;

        public BlogPostController(ApplicationDbContext context,
                                   IHostingEnvironment hostingEnvironment)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            this.hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
            uploadsPath = Path.Combine(hostingEnvironment.WebRootPath, "img");
        }

        // GET: BlogPosts
        public async Task<IActionResult> Index()
        {
            return View(await _context.BlogPosts.ToListAsync());
        }

        // GET: BlogPosts/Details/5
        [HttpGet("posts/{id}")]
        public async Task<IActionResult> Details(int? id, IFormFile newImage)
        {
            var keyAddres = "Ephraim";
            BlogPost blogPost = null;

            if (newImage != null)
            {
                var stream = new MemoryStream();
                await newImage.CopyToAsync(stream);
                var streamArray = stream.ToArray();
                var to64 = Convert.ToBase64String(streamArray);
                blogPost.Image = to64;
            }

            if (id != null)
            {
                blogPost = await GetBlogPostAsync(id);


                if (blogPost == null)
                {
                    return NotFound();
                }

                keyAddres = blogPost.Address;
            }

            var locationClient = new HttpClient();
            var mapClient = new HttpClient();
            if (keyAddres != null)
            {
                // beginning of my location api
                keyAddres = "410 South 300 East, Ephraim, Utah, 84627, United States";
                //Todo write catch that provides a good message for the times that result is empty or the api call is bad
                var locationResponse = await locationClient.GetStringAsync($"https://api.opencagedata.com/geocode/v1/json?q={keyAddres}&key=20ca7d134ab94eb5bab456e67d8038e6");
                var jObject = JObject.Parse(locationResponse);
                ViewData["lat"] = (string)jObject["results"][0]["geometry"]["lat"];
                ViewData["lng"] = (string)jObject["results"][0]["geometry"]["lng"];
                //beginning of my weather api
                var weatherClient = new HttpClient();
                string lat = (string)jObject["results"][0]["geometry"]["lat"];
                string lng = (string)jObject["results"][0]["geometry"]["lng"];
                var locationWeatherResponse = await weatherClient.GetStringAsync($"https://api.darksky.net/forecast/bd1108cdcba19b14fc8324d4d7f2231d/{lat},{lng}");
                var jObject1 = JObject.Parse(locationWeatherResponse);
                ViewData["curTemp"] = (string)jObject1["currently"]["temperature"];
                ViewData["futTemp"] = (string)jObject1["daily"]["data"][0]["temperatureHigh"];
                ViewData["cloud"] = (string)jObject1["daily"]["data"][0]["cloudCover"];
                ViewData["summ"] = (string)jObject1["daily"]["summary"];
                ////url for google map api that takes a lat and lng from the location api
                ViewData["map"] = $"https://www.google.com/maps/embed/v1/search?key=AIzaSyAYX22qQW38R2hxpLej-3cEpmQJjluw0vw&q={lat},{lng}";
            }

            return View("Details", blogPost);
        }

        private async Task<BlogPost> GetBlogPostAsync(int? id)
        {
            return await _context.BlogPosts
                           .Include(c => c.Comments)
                           .FirstOrDefaultAsync(m => m.Id == id);
        }

        // GET: BlogPosts/Create
        [Authorize(Policy = MyIdentityData.BlogPolicy_Add)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = MyIdentityData.BlogPolicy_Add)]
        public async Task<IActionResult> Create([Bind("Id,Title,Address,Start_Date,End_Date,Image,Trip_Detail,List,Directions")] BlogPost blogPost, IFormFile newImage)
        {
            if(newImage != null)
            {
                var stream = new MemoryStream();
                await newImage.CopyToAsync(stream);
                var streamArray = stream.ToArray();
                var to64 = Convert.ToBase64String(streamArray);
                blogPost.Image = to64;
            }

            if (ModelState.IsValid)
            {
                blogPost.Posted = DateTime.Now;
                _context.Add(blogPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        [Authorize(Policy = MyIdentityData.BlogPolicy_Edit)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }
            var view = View(blogPost);

            return view;
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = MyIdentityData.BlogPolicy_Edit)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Address,Start_Date,End_Date,Trip_Detail,Image,TagsString,List,Directions,Image")] BlogPost blogPost, IFormFile newImage)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (newImage != null)
                    {
                        var stream = new MemoryStream();
                        await newImage.CopyToAsync(stream);
                        var streamArray = stream.ToArray();
                        var to64 = Convert.ToBase64String(streamArray);
                        blogPost.Image = to64;
                    }

                    _context.Update(blogPost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(blogPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        [Authorize(Policy = MyIdentityData.BlogPolicy_Delete)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        [Authorize(Policy = MyIdentityData.BlogPolicy_Delete)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(int id)
        {
            return _context.BlogPosts.Any(e => e.Id == id);
        }

        [HttpPost("posts/{id}")]
        public async Task<IActionResult> AddComment(int blogPostId, string comment)
        {
            var c = new Comment();
            c.CommentText = comment;
            c.BlogPostId = blogPostId;
            await _context.Comments.AddAsync(c);
            await _context.SaveChangesAsync();
            //
            return RedirectToAction("Details", await GetBlogPostAsync(blogPostId));  
        }
    }
}