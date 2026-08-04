using Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class StokTakipDbContext : DbContext
    {
        public StokTakipDbContext(DbContextOptions<StokTakipDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
        public DbSet<ErrorMessage> ErrorMessages { get; set; }
    }
}