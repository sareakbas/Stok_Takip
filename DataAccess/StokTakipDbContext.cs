using Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class StokTakipDbContext : DbContext
    {
        public StokTakipDbContext(DbContextOptions<StokTakipDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Şifreni güncel docker şifrenle (Sare1234!) değiştirdim.
                optionsBuilder.UseSqlServer("Server=localhost,1433;Database=StokTakipDB;User Id=sa;Password=Sare1234!;TrustServerCertificate=True;");
            }
        }

        // İŞTE EKSİK OLAN VE HATAYI ÇÖZECEK METOT BURASI:
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cascade döngüsünü engellemek için silme davranışını 'Restrict' olarak ayarlıyoruz.
            modelBuilder.Entity<StockMovementAllocation>()
                .HasOne(sma => sma.Movement)
                .WithMany()
                .HasForeignKey(sma => sma.MovementId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StockMovementAllocation>()
                .HasOne(sma => sma.Lot)
                .WithMany()
                .HasForeignKey(sma => sma.LotId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<StockLot> StockLots { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<StockMovementAllocation> StockMovementAllocations { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
    }
}