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
        private ApplicationDbContext context = new ApplicationDbContext();
        public ActionResult Viewcomment()
        {
            var commentList=
            return View();
        }
        public ActionResult CreatCommentList() => RedirectToAction("Viewcomment", "Comments");
        public ActionResult CreatComment()
        {
            return View();
           
        }
    }
}