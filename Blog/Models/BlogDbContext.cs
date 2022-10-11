using Microsoft.EntityFrameworkCore;

namespace Blog.Models
{
    public class BlogDbContext: DbContext
    {
       public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
        {

        }

        public DbSet<BlogModel> Blogs { get; set; }
    }
}
