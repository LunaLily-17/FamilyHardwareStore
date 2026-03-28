using HardwareStore.Application.Abstractions;
using HardwareStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HardwareStore.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();
    public DbSet<StockAdjustment> StockAdjustments => Set<StockAdjustment>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<AppSetting> AppSettings => Set<AppSetting>();

    IQueryable<User> IAppDbContext.Users => Users;
    IQueryable<Category> IAppDbContext.Categories => Categories;
    IQueryable<Supplier> IAppDbContext.Suppliers => Suppliers;
    IQueryable<Product> IAppDbContext.Products => Products;
    IQueryable<Purchase> IAppDbContext.Purchases => Purchases;
    IQueryable<PurchaseItem> IAppDbContext.PurchaseItems => PurchaseItems;
    IQueryable<Sale> IAppDbContext.Sales => Sales;
    IQueryable<SaleItem> IAppDbContext.SaleItems => SaleItems;
    IQueryable<InventoryMovement> IAppDbContext.InventoryMovements => InventoryMovements;
    IQueryable<StockAdjustment> IAppDbContext.StockAdjustments => StockAdjustments;
    IQueryable<AuditLog> IAppDbContext.AuditLogs => AuditLogs;
    IQueryable<AppSetting> IAppDbContext.AppSettings => AppSettings;

    public async Task AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
        => await Set<TEntity>().AddAsync(entity, cancellationToken);

    void IAppDbContext.Update<TEntity>(TEntity entity)
        => base.Update(entity);

    public async Task<IAppDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        => new AppDbContextTransaction(await Database.BeginTransactionAsync(cancellationToken));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(x => x.Username).HasMaxLength(64).IsRequired();
            entity.Property(x => x.DisplayName).HasMaxLength(128).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
            entity.HasIndex(x => x.Username).IsUnique();
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(128).IsRequired();
            entity.Property(x => x.NameMm).HasMaxLength(128);
            entity.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Phone).HasMaxLength(50);
            entity.Property(x => x.Email).HasMaxLength(120);
            entity.HasIndex(x => x.Name);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.NameMm).HasMaxLength(200);
            entity.Property(x => x.Sku).HasMaxLength(64);
            entity.Property(x => x.Barcode).HasMaxLength(128);
            entity.Property(x => x.CostPrice).HasColumnType("TEXT");
            entity.Property(x => x.SalePrice).HasColumnType("TEXT");
            entity.Property(x => x.StockOnHand).HasColumnType("TEXT");
            entity.Property(x => x.ReorderLevel).HasColumnType("TEXT");
            entity.HasIndex(x => x.Name);
            entity.HasIndex(x => x.Sku).IsUnique();
            entity.HasIndex(x => x.Barcode);
            entity.HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.Property(x => x.PurchaseNumber).HasMaxLength(64).IsRequired();
            entity.HasIndex(x => x.PurchaseNumber).IsUnique();
            entity.HasIndex(x => x.PurchaseDateUtc);
            entity.HasOne(x => x.Supplier)
                .WithMany(x => x.Purchases)
                .HasForeignKey(x => x.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PurchaseItem>(entity =>
        {
            entity.HasOne(x => x.Purchase)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.PurchaseId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Product)
                .WithMany(x => x.PurchaseItems)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.Property(x => x.ReceiptNumber).HasMaxLength(64).IsRequired();
            entity.HasIndex(x => x.ReceiptNumber).IsUnique();
            entity.HasIndex(x => x.SaleDateUtc);
            entity.HasOne(x => x.Cashier)
                .WithMany()
                .HasForeignKey(x => x.CashierId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SaleItem>(entity =>
        {
            entity.HasOne(x => x.Sale)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.SaleId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Product)
                .WithMany(x => x.SaleItems)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InventoryMovement>(entity =>
        {
            entity.HasIndex(x => new { x.ProductId, x.MovementDateUtc });
            entity.HasOne(x => x.Product)
                .WithMany(x => x.InventoryMovements)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<StockAdjustment>(entity =>
        {
            entity.Property(x => x.Reason).HasMaxLength(500).IsRequired();
            entity.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.AdjustedByUser)
                .WithMany()
                .HasForeignKey(x => x.AdjustedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.Property(x => x.EntityName).HasMaxLength(128).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(1000).IsRequired();
            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AppSetting>(entity =>
        {
            entity.Property(x => x.Key).HasMaxLength(128).IsRequired();
            entity.Property(x => x.Value).HasMaxLength(2000).IsRequired();
            entity.HasIndex(x => x.Key).IsUnique();
        });
    }
}
