using Microsoft.EntityFrameworkCore;

namespace BoxTrackLabel.API.Models
{
    public class BoxTrackDbContext : DbContext
    {

        public BoxTrackDbContext(DbContextOptions<BoxTrackDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Storage>()
                .HasIndex(s => s.Codigo);
            modelBuilder.Entity<Shift>()
                .HasIndex(s => s.Codigo);
            modelBuilder.Entity<Process>()
                .HasIndex(s => s.Codigo);
            modelBuilder.Entity<Module>()
                .HasIndex(s => s.Codigo);
            modelBuilder.Entity<ConfigurationValue>()
                .HasIndex(c => c.Codigo);
            //    .IsUnique();
            
            //DataMatrix
            modelBuilder.Entity<Order>()
                .Property(o => o.Mrp)
                .HasDefaultValue("0000");
            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasDefaultValue("Creada");
            //Relaciones DataMatrix 
            modelBuilder.Entity<Order>()
                .HasMany<Code>(o => o.Codes)
                .WithOne(c => c.Order)
                .HasForeignKey(c => c.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        
        public DbSet<Productos_Prov> Productos_Prov { get; set; }
        public DbSet<Suplidores> Suplidores { get; set; }
        public DbSet<Productos> Productos { get; set; }
        public DbSet<Process> Processes { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Production> Productions { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<LabelConfig> LabelConfigs { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Storage> Storages { get; set; }
        public DbSet<Access> Accesses { get; set; }
        public DbSet<ConfigurationValue> ConfigurationValues {get; set;}
        
        //Modelos DataMatrix
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderSetting> OrderSettings { get; set; }
        public DbSet<Code> Codes { get; set; }
        public DbSet<EmailAccount> EmailAccounts { get; set; }

 
    }
}
