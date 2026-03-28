using System;
namespace FamilyHardwareStore.Api.Domain.Entities
{
	public class Category
    { 
        public int id { get; set; }
        public string Name { get; set; } = default!;

    public ICollection<Product> Products { get; set; } = new List<Product>();  
    }
}

