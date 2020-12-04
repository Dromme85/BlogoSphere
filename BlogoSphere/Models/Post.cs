using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlogoSphere.Models
{
	public class Post
	{
		public int Id { get; set; }

		[Required]
		[StringLength(100)]
		public string Title { get; set; }

		[Required]
		[StringLength(1024)]
		public string Body { get; set; }

		[Required]
		[DataType(DataType.DateTime)]
		public DateTime Created { get; set; }

		[Required]
		public int Views { get; set; }

		//public ICollection<Comment> Comment { get; set; }

		public virtual ICollection<Tag> Tags { get; set; }
	}
}