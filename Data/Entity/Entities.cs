using Core.Config;
using Microsoft.EntityFrameworkCore;

namespace Data.Entity
{
    public class Entities : DbContext
    {
        private readonly DbConfig _config;
        public Entities(DbConfig config)
        {
            _config = config;
        }
        public virtual DbSet<Basket> Basket { get; set; }
        public virtual DbSet<BasketProduct> BasketProduct { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Product> Product { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_config.DbConnectionString);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Basket>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UniqueId).IsRequired();
                entity.Property(e => e.CreateDate).IsRequired();
                entity.Property(e => e.StatusId).IsRequired();
            });

            modelBuilder.Entity<BasketProduct>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BasketId).IsRequired();
                entity.Property(e => e.ProductId).IsRequired();
                entity.Property(e => e.Count).IsRequired();
                entity.HasOne(d => d.Basket)
                    .WithMany(p => p.BasketProduct)
                    .HasForeignKey(d => d.BasketId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BasketProduct_Basket");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.CreateDate).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.UpdateDate).HasColumnType("datetime");
                entity.Property(e => e.Count).IsRequired();
                entity.Property(e => e.Status).IsRequired();
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Surname).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CreateDate).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            });

        }
    }
}
