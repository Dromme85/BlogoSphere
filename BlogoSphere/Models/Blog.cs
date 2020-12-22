using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlogoSphere.Models
{
    public class Blog
    {
        public int Id { get; set; }

        [Required]
        [StringLength(64)]
        public string Title { get; set; }

        [Required]
        [StringLength(1024)]
        public string Body { get; set; }

        [Required]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0: d MMMM yyyy, HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Created { get; set; }

        [Required]
        public int Views { get; set; }

        public virtual ICollection<Post> Posts { get; set; }

        public ApplicationUser Author { get; set; }
    }
}