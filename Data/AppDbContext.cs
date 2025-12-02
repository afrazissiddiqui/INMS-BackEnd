using Inventory.Api.Common;
using Inventory.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // DbSets
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<VAT> VATs { get; set; }
        public DbSet<SignUp> SignsUp { get; set; }
        /*  public DbSet<PendingApprovalRequest> PendingApprovalRequests { get; set; }*/

        public DbSet<BusinessPartner> BusinessPartners => Set<BusinessPartner>();
        public DbSet<Item> Items => Set<Item>();
        public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
        public DbSet<APInvoice> APInvoices => Set<APInvoice>();
        public DbSet<APInvoiceLine> APInvoiceLines => Set<APInvoiceLine>();
        public DbSet<ARInvoice> ARInvoices => Set<ARInvoice>();
        public DbSet<ARInvoiceLine> ARInvoiceLines => Set<ARInvoiceLine>();
        public DbSet<SalesOrder> SalesOrder { get; set; }
        public DbSet<SalesOrderLine> SalesOrderLines { get; set; }
        public DbSet<SalesOrderLineResponse> SalesOrderLinesResponse { get; set; }
        


        public DbSet<AssignPermission> AssignPermissions => Set<AssignPermission>();
        public DbSet<UnitOfMeasure> UnitsOfMeasure => Set<UnitOfMeasure>();
        /*public DbSet<ApprovalVote> ApprovalVotes { get; set; }*/

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Soft delete filter for all entities inheriting BaseEntity
            ApplySoftDeleteFilter<Role>(modelBuilder);
            ApplySoftDeleteFilter<User>(modelBuilder);
            ApplySoftDeleteFilter<Category>(modelBuilder);
            ApplySoftDeleteFilter<BusinessPartner>(modelBuilder);
            ApplySoftDeleteFilter<Item>(modelBuilder);
            ApplySoftDeleteFilter<StockTransaction>(modelBuilder);
            ApplySoftDeleteFilter<APInvoice>(modelBuilder);
            ApplySoftDeleteFilter<APInvoiceLine>(modelBuilder);
            ApplySoftDeleteFilter<ARInvoice>(modelBuilder);
            ApplySoftDeleteFilter<ARInvoiceLine>(modelBuilder);
            ApplySoftDeleteFilter<UnitOfMeasure>(modelBuilder);
            ApplySoftDeleteFilter<VAT>(modelBuilder);

            // ✅ Relations
            // Item → Category
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Category)
                .WithMany()
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Item → UnitOfMeasure
            modelBuilder.Entity<Item>()
                .HasOne(i => i.UnitOfMeasure)
                .WithMany()
                .HasForeignKey(i => i.UnitOfMeasureId)
                .OnDelete(DeleteBehavior.Restrict);

            // APInvoice → BusinessPartner
            modelBuilder.Entity<APInvoice>()
                .HasOne(i => i.BusinessPartner)
                .WithMany()
                .HasForeignKey(i => i.BusinessPartnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // APInvoiceLine → Item
            modelBuilder.Entity<APInvoiceLine>()
                .HasOne(l => l.Item)
                .WithMany()
                .HasForeignKey(l => l.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // APInvoiceLine → APInvoice
            modelBuilder.Entity<APInvoiceLine>()
                .HasOne(l => l.APInvoice)
                .WithMany(i => i.Lines)
                .HasForeignKey(l => l.APInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // ARInvoice → BusinessPartner
            modelBuilder.Entity<ARInvoice>()
                .HasOne(i => i.BusinessPartner)
                .WithMany()
                .HasForeignKey(i => i.BusinessPartnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ARInvoiceLine → Item
            modelBuilder.Entity<ARInvoiceLine>()
                .HasOne(l => l.Item)
                .WithMany()
                .HasForeignKey(l => l.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // ARInvoiceLine → ARInvoice
            modelBuilder.Entity<ARInvoiceLine>()
                .HasOne(l => l.ARInvoice)
                .WithMany(i => i.Lines)
                .HasForeignKey(l => l.ARInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Precision configs
            modelBuilder.Entity<StockTransaction>()
                .Property(t => t.Qty)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ARInvoiceLine>()
                .Property(l => l.UnitPrice)
                .HasColumnType("decimal(18,2)");
            
            modelBuilder.Entity<APInvoiceLine>()
                .Property(l => l.UnitPrice)
                .HasColumnType("decimal(18,2)");

            /*modelBuilder.Entity<ApprovalDetail>()
               .HasOne(a => a.PendingApprovalRequest)
               .WithMany(r => r.ApprovalDetails)
               .HasForeignKey(a => a.PendingApprovalRequestId)
               .OnDelete(DeleteBehavior.Cascade);*/


        }

        // ✅ Soft delete filter
        private static void ApplySoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
            where TEntity : BaseEntity
        {
            modelBuilder.Entity<TEntity>()
                .HasQueryFilter(e => !e.IsDeleted);
        }

        // ✅ Auto timestamps
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        entry.Entity.IsDeleted = false;
                        entry.Entity.IsActive = true;
                        entry.Entity.UpdatedAt = null;
                        break;

                    case EntityState.Modified:
                        entry.Property(e => e.CreatedAt).IsModified = false; // don’t touch CreatedAt
                        entry.Entity.UpdatedAt = now;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}