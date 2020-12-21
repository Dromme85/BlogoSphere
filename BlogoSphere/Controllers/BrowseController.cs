using BlogoSphere.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogoSphere.Controllers
{
    public class BrowseController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Browse
        public ActionResult BrowseViews()
        {
            var blogs = db.Blogs.OrderByDescending(v=>v.Views).ToList();
            if (blogs != null)
                return View(blogs);
            return View(blogs);
        }

        public ActionResult BrowseTagTabs()
        {
            ViewBag.PopularTags = db.Tags.ToList();
            Session["TagsToAdd"] = new List<Tag>();

            //var PoularTags = db.Tags.ToList();
            //return View(PoularTags);

            return View(ViewBag.PopularTags);
        }
        
        public ActionResult BrowseTags()
        {
            var TagResult = db.Posts.ToList();
            return View(TagResult);
        }
    }
}