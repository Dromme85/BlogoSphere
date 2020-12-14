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

            ViewBag.CurrentBlogId = GetCurrentBlogId((int)postId);
            var post = db.Posts.Include(p => p.Tags).Where(p => p.Id == postId).FirstOrDefault();

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

        public ActionResult List(int? blogId)
		{
            if (blogId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ViewBag.BlogId = blogId;

            var model = db.Blogs.Include(b => b.Posts).Where(b => b.Id == blogId).Select(p => p.Posts).ToList();

            if (model.Count == 0 || model == null)
                return RedirectToAction("Index", "Home");

            return View(model.First());
		}

        public ActionResult Create(int? blogId)
		{
            if (blogId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Session["BlogId"] = blogId;

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
                model.Comments = new List<Comment>();

                var tta = (List<Tag>)Session["TagsToAdd"];

                db.Posts.Add(model);
                db.SaveChanges();

                int pId = db.Posts.OrderByDescending(m => m.Id).First().Id;
				foreach (var tag in tta)
				{
                    AddTags(pId, tag.Name);
				}

                var user = db.Users.Find(User.Identity.GetUserId());
                var blogid = (int)Session["BlogId"];
                var blog = db.Blogs.Find(blogid);
                if (blog == null)
                    return HttpNotFound();

                blog.Posts.Add(db.Posts.Find(pId));
                db.SaveChanges();

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

                return RedirectToAction("Index", new { postid = model.Id });
			}

            ViewBag.PopularTags = db.Tags.Take(10).ToList();

            return View(model);
		}

        public JsonResult AttachTag(string name)
		{
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

        private int GetCurrentBlogId(int? postId)
		{
            return db.Blogs.Include(b => b.Posts).Where(b => b.Posts.Any(p => p.Id == postId)).First().Id;
		}
    }
}