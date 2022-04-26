using Microsoft.EntityFrameworkCore;

namespace BoxTrackLabel.API.Models
{
    public class MaestrosDbContext: DbContext
    {
    
        public MaestrosDbContext(DbContextOptions<MaestrosDbContext> options)
            : base(options)
        {
             
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            modelBuilder.Entity<RolPermission>()
                .HasKey(R => new { R.IdPermiso, R.IdRol });
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RolPermission> RolPermissions { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Product> Products {get; set;} 
        public DbSet<Customer> Customers {get; set;}
    }
}
