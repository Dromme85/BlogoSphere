using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlogoSphere.Models
{ 
        public class Comment
        {
            public int Id { get; set; }
            [Required]
            [StringLength(512)]
            public string Body { get; set; }

            [Required]
            [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0: d MMMM yyyy, HH:mm}", ApplyFormatInEditMode = true)]
            public DateTime Created { get; set; }

            public string UserId { get; set; }
            public ApplicationUser User { get; set; }

            public int PostId { get; set; }
            public Post Post { get; set; }
        }
    }