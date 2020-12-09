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
    [Authorize]
    public class CommentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [AllowAnonymous]
        public ActionResult Display()
        {
            var Comments = db.Comments.ToList().Take(5);
            return View(Comments);
        }
        
        public ActionResult Create()
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

                return RedirectToAction("Create");
            }

            return View();
        }
    }
}