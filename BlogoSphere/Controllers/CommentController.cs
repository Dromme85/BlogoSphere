using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlogoSphere.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BlogoSphere.Controllers
{
    public class CommentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult _View()
        {
                return View(db.Comments.ToList());
        }
        public ActionResult CreateCommentList() => RedirectToAction("_View", "Comments");

        public ActionResult _Create()
        {
            return View();           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Body")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                comment.Created = DateTime.Now;


                db.SaveChanges();
                ViewBag.message = "Comment added Successfully..!";

                return RedirectToAction("Index");
            }

            return View(_View());
        }
    }
}