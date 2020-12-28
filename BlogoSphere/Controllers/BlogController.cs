using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BlogoSphere.Models;
using Microsoft.AspNet.Identity;

namespace BlogoSphere.Controllers
{
    [Authorize]
    public class BlogController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        static int bv = 0;
        // GET: Blogs
        public ActionResult Index()
        {
            //var blogs = new List<Blog>();
            var uid = User.Identity.GetUserId();
            var blogs = db.Blogs.Include(b => b.Author).Where(b => b.Author.Id == uid).ToList();
        
            if (blogs != null)
                return View(blogs);

            return View();
        }

        // GET: Blogs/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id, int? year, int? month)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Include(b => b.Author).Where(b => b.Id == id).FirstOrDefault();
            if (blog == null)
            {
                return HttpNotFound();
            }

            if (year != null)
			{
                if (month != null)
				{
                    // Get all posts of 'year/month'
                    DateTime ym = new DateTime((int)year, (int)month, 01);
                    var posts = db.Posts.Where(p => p.Created.Year == ym.Year && p.Created.Month == ym.Month && p.BlogId == id).ToList();
                    ViewBag.DatePosts = posts;
				}
				else
                {
                    // Get all posts of 'year'
                    DateTime y = new DateTime((int)year, 01, 01);
                    var posts = db.Posts.Where(p => p.Created.Year == y.Year && p.BlogId == id).ToList();
                    ViewBag.DatePosts = posts;
                }
			}

            if (Session[blog.Id + "views"] == null)
                bv = blog.Views + 1;
            else
                bv = (int)Session[blog.Id + "views"] + 1;
            Session[blog.Id + "views"] = bv;

            blog.Views = (int)Session[blog.Id + "views"];
            db.Entry(blog).State = EntityState.Modified;
            db.SaveChanges();
            
            return View(blog);
        }

        // GET: Blogs/Create
        public ActionResult Create()
        {
            Blog model = new Blog();

            return View(model);
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Body")] Blog blog)
        {
            if (ModelState.IsValid)
            {
                blog.Created = DateTime.Now;
                blog.Views = 0;
                db.Blogs.Add(blog);
                db.Users.Find(User.Identity.GetUserId()).Blogs.Add(blog);
                db.SaveChanges();
                ViewBag.message = "Blog Details are saved Successfully..!";


                return RedirectToAction("Index");
            }

            return View(blog);
        }

        // GET: Blogs/Edit/5
        //public ActionResult DisplayBlogFor(string CheckoutAddressBox)
        //{

        //    //var displayBlog = db.Blogs.Where(b => b.Title.Contains(CheckoutAddressBox)).ToList();
        //    //if (displayBlog == null)
        //    //    return RedirectToAction("Index", "Home");

        //    return View(db.Blogs.Where(b => b.Title.Contains(CheckoutAddressBox))  CheckoutAddressBox ==null).ToList());
        //}

        [AllowAnonymous]
        public ActionResult DisplayBlogPostFor(string SearchBox)
        {
            SearchVM obj = new SearchVM();

            if (SearchBox != null)
            {
                obj.BlogList = db.Blogs.Include(b => b.Author).Where(b =>  b.Title.Contains(SearchBox)).ToList();
                obj.PostList = db.Posts.Include(p => p.Blog).Include(p => p.Blog.Author).Where(p =>  p.Title.Contains(SearchBox)).ToList();
                obj.SearchText = SearchBox;
            }

            return View(obj);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Body")] Blog blog)
        {
            var original_data = db.Blogs.AsNoTracking().FirstOrDefault();
            if (ModelState.IsValid)
            {
                db.Entry(blog).State = EntityState.Modified;
                blog.Created = original_data.Created;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blog);
        }

        // GET: Blogs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Include(b => b.Author).Where(b => b.Id == id).FirstOrDefault();
            if (blog == null)
            {
                return HttpNotFound();
            }
            if (blog.Author.Id != User.Identity.GetUserId())
                return RedirectToAction("Details", "Blog", blog.Id);

            return View(blog);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Blog blog = db.Blogs.Find(id);
            db.Blogs.Remove(blog);
            db.SaveChanges();
            return RedirectToAction("BrowseViews", "Browse");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        

    }
}
