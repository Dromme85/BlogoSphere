using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogoSphere.Models
{
    public class SearchVM
    {
        public List<Blog> BlogList { get; set; }
        public List<Post> PostList { get; set; }
        public string SearchText { get; set; }
        public SearchVM()
        {
            BlogList = new List<Blog>();
            PostList = new List<Post>();
        }


    }
}