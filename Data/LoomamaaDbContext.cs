using LoomamaaApp.Klassid;
using Microsoft.EntityFrameworkCore;

namespace LoomamaaApp.Data
{
    public class LoomamaaDbContext : DbContext
    {
        public LoomamaaDbContext(DbContextOptions<LoomamaaDbContext> options)
            : base(options)
        {
        }

        public DbSet<Animal> Animals { get; set; }
        public DbSet<EnclosureEntity> Enclosures { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Animal entity with TPH (Table Per Hierarchy) inheritance
            modelBuilder.Entity<Animal>(entity =>
            {
                entity.ToTable("Animals");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Age).IsRequired();
                entity.Property(e => e.EnclosureId).IsRequired(false);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql(null);
                entity.Property(e => e.CreatedDate).HasDefaultValue(System.DateTime.Now);
                
                // Ignore computed property TypeName - it's derived from the class type
                entity.Ignore(e => e.TypeName);
                
                // Use discriminator for inheritance with column named "Type"
                entity.HasDiscriminator<string>("Type")
                    .HasValue<Cat>("Cat")
                    .HasValue<Dog>("Dog")
                    .HasValue<Bear>("Bear")
                    .HasValue<Duck>("Duck")
                    .HasValue<Horse>("Horse")
                    .HasValue<Monkey>("Monkey")
                    .HasValue<Pig>("Pig")
                    .HasValue<Sheep>("Sheep");
            });

            // Configure Cat entity
            modelBuilder.Entity<Cat>();
            
            // Configure Dog entity
            modelBuilder.Entity<Dog>();
            
            // Configure Bear entity
            modelBuilder.Entity<Bear>();
            
            // Configure Duck entity
            modelBuilder.Entity<Duck>();
            
            // Configure Horse entity
            modelBuilder.Entity<Horse>();
            
            // Configure Monkey entity
            modelBuilder.Entity<Monkey>();
            
            // Configure Pig entity
            modelBuilder.Entity<Pig>();
            
            // Configure Sheep entity
            modelBuilder.Entity<Sheep>();

            // Configure Enclosure entity
            modelBuilder.Entity<EnclosureEntity>(entity =>
            {
                entity.ToTable("Enclosures");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql(null);
                entity.Property(e => e.CreatedDate).HasDefaultValue(System.DateTime.Now);
            });
        }
    }

    // Entity for Enclosure storage
    public class EnclosureEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public System.DateTime CreatedDate { get; set; }
    }
}
