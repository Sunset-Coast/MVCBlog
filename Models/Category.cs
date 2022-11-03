using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechnicalBlog.Models
{
    public class Category
    {
        //Primary Key
        public int Id { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
        public string? Name { get; set; }
        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
        public string? Description { get; set; }

        public byte[]? ImageData { get; set; }
        public string? ImageType { get; set; }
        [NotMapped]
        public IFormFile? CategoryImage { get; set; }


        //Navigation Property
        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new HashSet<BlogPost>();

    }
}
