namespace Catalog.Application.DbModels
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public string Package { get; set; }
        public int Assets { get; set; }
    }
}