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

namespace BlogoSphere.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [AllowAnonymous]
        public ActionResult Display()
        {
            var Comments = db.Comments.Take(5).ToList();

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
            }
            return View();
        }
        // GET: Comments/Edit/5
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

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Body")] Comment comment, Uri previousUrl)
        {           
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                comment.Created = DateTime.Now;
                db.SaveChanges();                
            }
            return Redirect(previousUrl.ToString());
            //return View();
        }

        // GET: Comments/Delete/5
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

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, Uri previousUrl)
        {       
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
            db.SaveChanges();
            return Redirect(previousUrl.ToString());
            //return View();
        }
    }
}