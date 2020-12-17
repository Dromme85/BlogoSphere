using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogoSphere.Models
{
    public class BlogPostVM
    {
        public List<Blog> BlogList { get; set; }
        public List<Post> PostList { get; set; }
        public String searching { get; set; }
        public BlogPostVM()
        {
            BlogList = new List<Blog>();
            PostList = new List<Post>();
        }


    }
}