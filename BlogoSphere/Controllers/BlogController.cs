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

        // GET: Blogs
        [AllowAnonymous]
        public ActionResult Index()
        {
        
            return View(db.Blogs.ToList());
        }

        // GET: Blogs/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
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

        // GET: Blogs/Create
        public ActionResult Create()
        {
            return View();
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
        public ActionResult DisplayBlogPostFor(string CheckoutAddressBox)
        {
            BlogPostVM obj = new BlogPostVM();
             if (CheckoutAddressBox != null)
            {
                obj.BlogList = db.Blogs.Where(b =>  b.Title.Contains(CheckoutAddressBox)).ToList();
                obj.PostList = db.Posts.Where(p =>  p.Title.Contains(CheckoutAddressBox)).ToList();
                obj.searching = CheckoutAddressBox;
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
            if (ModelState.IsValid)
            {
                db.Entry(blog).State = EntityState.Modified;
                blog.Created = DateTime.Now;
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
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
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
            return RedirectToAction("Index");
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
