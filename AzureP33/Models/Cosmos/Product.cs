namespace AzureP33.Models.Cosmos
{
    public class Product
    {
        public Guid id { get; set; }
        public string categoryId { get; set; } = null!;
        public String categoryName { get; set; } = null!;
        public String sku { get; set; } = null!;
        public String name { get; set; } = null!;
        public String description { get; set; } = null!;
        public double price { get; set; }
    }
}