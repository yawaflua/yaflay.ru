using Microsoft.EntityFrameworkCore;
using yaflay.ru.Database.Tables;
using yaflay.ru.Models.Tables;

namespace yaflay.ru.Models
{
    public class AppDbContext : DbContext
    {
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<Blogs> Blogs { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<Redirects> Redirects { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
