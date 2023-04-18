using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HFiles.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<UserFile> UserFiles { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

        }
    }
}