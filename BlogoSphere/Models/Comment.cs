using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlogoSphere.Models
{
    public class Comment
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Body { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
    }
}