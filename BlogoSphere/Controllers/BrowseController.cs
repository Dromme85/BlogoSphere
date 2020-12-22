using BlogoSphere.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace BlogoSphere.Controllers
{
    public class BrowseController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Browse
        public ActionResult BrowseViews()
        {
            var blogs = db.Blogs.Include(b => b.Author).OrderByDescending(v => v.Views).ToList();
            if (blogs != null)
                return View(blogs);
            return View(blogs);
        }

        public ActionResult BrowseTagTabs()
        {           
            var PoularTags = db.Tags.ToList();
            return View(PoularTags);           
        }
        
        public ActionResult BrowseTags()
        {
            var TagResult = db.Posts.Include(p => p.Blog).Include(p=>p.Blog.Author).ToList();
            return View(TagResult);
        }
    }
}