using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models
{
    public class BlogModel
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Title { get; set; }


        public string Description { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string? ImageName { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }


    }
}
