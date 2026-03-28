using HardwareStore.Domain.Entities;

namespace HardwareStore.Application.Abstractions;

public interface IAppDbContext
{
    IQueryable<User> Users { get; }
    IQueryable<Category> Categories { get; }
    IQueryable<Supplier> Suppliers { get; }
    IQueryable<Product> Products { get; }
    IQueryable<Purchase> Purchases { get; }
    IQueryable<PurchaseItem> PurchaseItems { get; }
    IQueryable<Sale> Sales { get; }
    IQueryable<SaleItem> SaleItems { get; }
    IQueryable<InventoryMovement> InventoryMovements { get; }
    IQueryable<StockAdjustment> StockAdjustments { get; }
    IQueryable<AuditLog> AuditLogs { get; }
    IQueryable<AppSetting> AppSettings { get; }

    Task AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class;
    void Update<TEntity>(TEntity entity) where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IAppDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
