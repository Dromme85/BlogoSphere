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

        [AllowAnonymous]
        public ActionResult Index(int? postId)
        {
            if (postId == null) postId = 1;

            var post = db.Posts.Include(p => p.Tags).Where(p => p.Id == postId).FirstOrDefault();

            // TODO: make this work
            post.Views++;

            return View(post);
        }

        public ActionResult List(int? blogId)
		{
            if (blogId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var model = db.Blogs.Include(b => b.Post).Where(b => b.Id == blogId).Select(p => p.Post).ToList();

            if (model.Count == 0 || model == null)
                return RedirectToAction("Index", "Home");

            return View(model.First());
		}

        public ActionResult Create()
		{
            // TODO: Make PopularTags work.
            ViewBag.PopularTags = db.Tags.Take(10).ToList();
            Session["TagsToAdd"] = new List<Tag>();

            return View();
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Body")] Post model) // Add image here later.
		{
            if (ModelState.IsValid)
			{
                model.Created = DateTime.Now;
                model.Views = 0;
                model.Comment = new List<Comment>();

                var tta = (List<Tag>)Session["TagsToAdd"];

                db.Posts.Add(model);
                db.SaveChanges();

                int pId = db.Posts.OrderByDescending(m => m.Id).First().Id;
				foreach (var tag in tta)
				{
                    AddTags(pId, tag.Name);
				}

                // TODO: Add post id to current blog, somehow

                // TODO: Should take you directly to the new post
                return RedirectToAction("Index", new { postid = pId });
            }

            ViewBag.PopularTags = db.Tags.Take(10).ToList();

            return View(model);
		}

        public ActionResult Edit(int? postId)
		{
            if (postId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Post model = db.Posts.Include(p => p.Tags).Where(p => p.Id == postId).FirstOrDefault();
            if (model == null)
                return HttpNotFound();

            ViewBag.PopularTags = db.Tags.Take(10).ToList();
            Session["TagsToAdd"] = model.Tags.ToList();

            return View(model);
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Body,Created,Comment,Tags,Views")] Post model)
		{
            if (ModelState.IsValid)
			{
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

                // TODO: Should redirect to the edited post
                return RedirectToAction("Index", "Home");
			}

            ViewBag.PopularTags = db.Tags.Take(10).ToList();

            return View(model);
		}

        public JsonResult AttachTag(string name)
		{
            // TODO: Check if any illegal characters are in the name. If so, don't add the tag.
            // (only alphabetic and numeric characters should be allowed)
            var tta = (List<Tag>)Session["TagsToAdd"];

            if (!tta.Any(t => t.Name == name) && name.Count() > 2)
			{
                tta.Add(new Tag() { Name = name });
                Session["TagsToAdd"] = tta;
			}

            return Json(tta, JsonRequestBehavior.AllowGet);
		}

        public void AddTags(int postId, string name)
		{
            Post post = db.Posts.Find(postId);

            if (!db.Tags.Any(t => t.Name == name))
			{
                Tag tag = new Tag() { Name = name };
                tag.Post = new List<Post>();
                tag.Post.Add(post);

                db.Tags.Add(tag);
			}
            else
			{
                var tag = db.Tags.Where(t => t.Name == name).First();
                tag.Post.Add(post);
			}

            db.SaveChanges();
		}

        public void DetachTag(int postId, string name)
        {
            Post post = db.Posts.Find(postId);

            if (post.Tags.Any(t => t.Name == name))
            {
                post.Tags.Remove(post.Tags.First(t => t.Name == name));

                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
            }
            else ViewBag.TagError = "No tag by that name. Cannot remove.";
		}
    }
}