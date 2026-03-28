namespace HardwareStore.Application.DTOs.Suppliers;

public sealed class SupplierDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;
}
