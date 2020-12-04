using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlogoSphere.Models
{
	public class Tag
	{
		public int Id { get; set; }

		[Required]
		[StringLength(64)]
		public string Name { get; set; }


		public virtual ICollection<Post> Post { get; set; }
	}
}