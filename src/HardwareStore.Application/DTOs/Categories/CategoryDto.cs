namespace HardwareStore.Application.DTOs.Categories;

public sealed class CategoryDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameMm { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}
