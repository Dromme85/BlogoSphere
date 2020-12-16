using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BlogoSphere.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Migrations;

namespace BlogoSphere.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
       
        [AllowAnonymous]
        public ActionResult Display(int? postId, string userName)
        {
            
            if (postId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);            
            
            var comments = db.Comments.Include(c => c.Post).Include(c => c.User).OrderByDescending(c => c.Created).ToList();

            return View(comments.Take(5));
        }
        
        public ActionResult Create(int? postId)
        {           
            if (postId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Session["PostId"] = postId;
            
            return View();           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Body")] Comment comment, Uri previousUrl)
        {
            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                comment.Created = DateTime.Now;
                db.Posts.Find((int)Session["PostId"]).Comments.Add(comment);
                db.Users.Find(User.Identity.GetUserId()).Comments.Add(comment);
                db.SaveChanges();
                Response.Redirect(Request.Url.ToString(), false);
            }         
            ModelState.Clear();
            return View();
        }
       
        public ActionResult Edit(int? id)
        {
            ViewBag.PreviousURL = Request.UrlReferrer;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }           
            return View(comment);
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Body")] Comment comment, Uri previousUrl)
        {           
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                comment.Created = DateTime.Now;
                db.Posts.Find((int)Session["PostId"]).Comments.Add(comment);
                db.Users.Find(User.Identity.GetUserId()).Comments.Add(comment);
                db.SaveChanges();                
            }
            return Redirect(previousUrl.ToString());           
        }
       
        public ActionResult Delete(int? id)
        {
            ViewBag.PreviousURL = Request.UrlReferrer;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View();
        }
      
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, Uri previousUrl)
        {       
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
            db.SaveChanges();
            return Redirect(previousUrl.ToString());           
        }
    }
}