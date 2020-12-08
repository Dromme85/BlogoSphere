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
        public ActionResult Viewcomment()
        {
                return View();
        }
        public ActionResult CreateCommentList() => RedirectToAction("Viewcomment", "Comments");
        public ActionResult CreateComment()
        {
            return View();
           
        }
    }
}