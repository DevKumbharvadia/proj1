using AppAPI.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace AppAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        #region DbSets
        public DbSet<Product> Products { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserAudit> UserAudits { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<DeletedProduct> DeletedProducts { get; set; }
        public DbSet<BlacklistedUser> BlacklistedUsers { get; set; }
        public DbSet<UserAction> UserActions { get; set; }
        public DbSet<BuyerInfo> BuyerInfos { get; set; }
        public DbSet<ProductStockLog> ProductStockLogs { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region User Configuration
            modelBuilder.Entity<User>()
                .HasMany(u => u.Products)
                .WithOne(p => p.Seller)
                .HasForeignKey(p => p.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.RefreshTokens)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.UserAudits)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Transactions)
                .WithOne(t => t.Buyer)
                .HasForeignKey(t => t.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Transaction Configuration
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Product)
                .WithMany(p => p.Transactions)
                .HasForeignKey(t => t.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Buyer)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Seller)
                .WithMany()
                .HasForeignKey(t => t.SellerId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region UserRole Configuration
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Product Configuration
            modelBuilder.Entity<Product>()
                .Property(p => p.StockQuantity)
                .HasDefaultValue(0);

            modelBuilder.Entity<Product>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Product>()
                .Property(p => p.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
            #endregion

            #region BlacklistedUser Configuration
            modelBuilder.Entity<BlacklistedUser>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region DeletedProduct Configuration
            modelBuilder.Entity<DeletedProduct>()
                .HasOne(d => d.Product)
                .WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region BuyerInfo Configuration
            modelBuilder.Entity<BuyerInfo>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region ProductStockLog Configuration
            modelBuilder.Entity<ProductStockLog>()
                .HasOne(psl => psl.Product)
                .WithMany()
                .HasForeignKey(psl => psl.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region UserAudit Configuration
            modelBuilder.Entity<UserAudit>()
               .HasOne(ua => ua.User)
               .WithMany(u => u.UserAudits)
               .HasForeignKey(ua => ua.UserId)
               .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region UserAction Configuration
            modelBuilder.Entity<UserAction>()
               .HasOne(ua => ua.UserAudit)
               .WithMany()
               .HasForeignKey(ua => ua.UserAuditId)
               .OnDelete(DeleteBehavior.Restrict);
            #endregion
        }

    }
}
