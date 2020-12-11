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

		[DataType(DataType.ImageUrl)]
		public string Image { get; set; }

		public virtual ICollection<Comment> Comments { get; set; }

		public virtual ICollection<Tag> Tags { get; set; }
	}
}