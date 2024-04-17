using api.yawaflua.ru.Models.Tables;
using Microsoft.EntityFrameworkCore;
using yawaflua.ru.Database.Tables;
using yawaflua.ru.Models.Tables;

namespace yawaflua.ru.Models
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
        public DbSet<Projects> Projects { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
