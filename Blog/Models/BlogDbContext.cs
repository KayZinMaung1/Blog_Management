using Microsoft.EntityFrameworkCore;

namespace Blog.Models
{
    public class BlogDbContext: DbContext
    {
       public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
        {

        }

        public DbSet<BlogModel> Blogs { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken= default)
        {
            var entries = this.ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

            foreach(var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).UpdatedDate = DateTime.Now;
                if(entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedDate = DateTime.Now;
                }
               
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
