using BlogoSphere.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace BlogoSphere.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Post
        static int pv = 0;

        [AllowAnonymous]
        public ActionResult Index(int? postId)
        {
            if (postId == null) return RedirectToAction("Index", "Blog" );

            var post = db.Posts.Include(p => p.Tags).Include(p => p.Blog).Include(p => p.Blog.Author).Where(p => p.Id == postId).FirstOrDefault();

            // Get UserName for current blog and compare it to the current logged in users name
            bool isOwner = false;
            if (post.Blog.Author.UserName == User.Identity.Name) isOwner = true;

            ViewBag.CurrentBlogId = post.BlogId;
            ViewBag.IsOwner = isOwner;

            if (Session[post.Id + "views"] == null)
                pv = post.Views + 1;
			else
                pv = (int)Session[post.Id + "views"] + 1;
            Session[post.Id + "views"] = pv;

            post.Views = (int)Session[post.Id + "views"];
            db.Entry(post).State = EntityState.Modified;
            db.SaveChanges();

            return View(post);
        }

        [AllowAnonymous]
        public ActionResult List(int? blogId)
		{
            if (blogId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ViewBag.BlogId = blogId;

            //var model = db.Blogs
            //    .Include(b => b.Posts)
            //    .Include(b => b.Author)
            //    .Where(b => b.Id == blogId)
            //    .Select(p => p.Posts).First().ToList();

            var model = db.Posts
                .Include(p => p.Blog)
                .Include(p => p.Blog.Author)
                .Where(p => p.BlogId == blogId).ToList();

            if (model.Count == 0 || model == null)
                return View();

            return View(model);
		}

        public ActionResult Create(int? blogId)
		{
            if (blogId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Post model = new Post() { BlogId = (int)blogId };

            // TODO: Make PopularTags work.
            ViewBag.PopularTags = db.Tags.Take(10).ToList();
            Session["TagsToAdd"] = new List<Tag>();

            return View(model);
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Body,BlogId")] Post model) // Add image here later.
		{
            if (ModelState.IsValid)
			{
                model.Created = DateTime.Now;
                model.Views = 0;
                model.Comments = new List<Comment>();

                var tta = (List<Tag>)Session["TagsToAdd"];
                model.Tags = tta;

                db.Posts.Add(model);
                db.SaveChanges();

                int pId = db.Posts.OrderByDescending(p => p.Id).First().Id;

                return RedirectToAction("Index", new { postid = pId });
            }

            ViewBag.PopularTags = db.Tags.Take(10).ToList();

            return View(model);
		}

        public ActionResult Edit(int? postId)
		{
            if (postId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            using (var tempDb = new ApplicationDbContext())
            {
                Post model = tempDb.Posts.Include(p => p.Tags).Where(p => p.Id == postId).FirstOrDefault();
                if (model == null)
                    return HttpNotFound();

                ViewBag.PopularTags = db.Tags.Take(10).ToList();

                Session["TagsToAdd"] = new List<Tag>();

				foreach (var item in model.Tags)
				{
					AttachTag(item.Name);
				}

				return View(model);
            }
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Body,Created,Views,Image")] Post model)
		{
            if (ModelState.IsValid)
            {
                using (var tempDb = new ApplicationDbContext())
                {
                    Post post = tempDb.Posts.Find(model.Id);

                    var newTags = (List<Tag>)Session["TagsToAdd"];
                    var oldTags = post.Tags.ToList();

					foreach (var item in oldTags)
					{
                        if (!newTags.Any(t => t.Name == item.Name))
                            post.Tags.Remove(item);
					}

					foreach (var item in newTags)
					{
                        if (!oldTags.Any(t => t.Name == item.Name))
                            post.Tags.Add(tempDb.Tags.Where(t => t.Name == item.Name).FirstOrDefault() ?? item);
					}

                    tempDb.Entry(post).State = EntityState.Modified;
                    tempDb.SaveChanges();
				}

                return RedirectToAction("Index", new { postid = model.Id });
			}

            ViewBag.PopularTags = db.Tags.Take(10).ToList();

            return View(model);
		}

        public JsonResult AttachTag(string name)
		{
            var tta = (List<Tag>)Session["TagsToAdd"];
            if (!tta.Any(t => t.Name == name) && name.Length > 2)
            {
				if (db.Tags.Any(t => t.Name == name))
				{
					var tag = db.Tags.Where(t => t.Name == name).First();
					tta.Add(new Tag() { Name = name, Id = tag.Id });
				}
				else
					tta.Add(new Tag() { Name = name });

                Session["TagsToAdd"] = tta;
            }

            return Json(tta, JsonRequestBehavior.AllowGet);
		}

        public JsonResult DetachTag(string name)
        {
            var tta = (List<Tag>)Session["TagsToAdd"];

            if (tta.Any(t => t.Name == name))
            {
                tta.Remove(tta.First(t => t.Name == name));
                Session["TagsToAdd"] = tta;
            }
            else
                ViewBag.TagError = "No tag by that name. Cannot remove.";

            return Json(tta, JsonRequestBehavior.AllowGet);
		}

        public void AddTags(int postId, string name)
        {
            Post post = db.Posts.Find(postId);

            if (!db.Tags.Any(t => t.Name == name))
            {
                Tag tag = new Tag() { Name = name };
                tag.Posts = new List<Post>();
                tag.Posts.Add(post);

                db.Tags.Add(tag);
            }
            else
            {
                var tag = db.Tags.Where(t => t.Name == name).First();
                tag.Posts.Add(post);
            }

            db.SaveChanges();
        }


        public void DeleteTag(int postId, string name)
        {
            Post post = db.Posts.Find(postId);

			if (post.Tags.Any(t => t.Name == name))
			{
				var tag = db.Tags.Where(t => t.Name == name).First();
				tag.Posts.Remove(post);

				db.Entry(post).State = EntityState.Modified;
				db.SaveChanges();
			}
			else
				ViewBag.TagError = "No tag by that name. Cannot remove.";
        }

        private int GetCurrentBlogId(int? postId)
		{
            return db.Blogs.Include(b => b.Posts).Where(b => b.Posts.Any(p => p.Id == postId)).First().Id;
		}
    }
}