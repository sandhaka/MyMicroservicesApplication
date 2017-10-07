namespace Orders.Application.Controllers
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        
        public string ProductName { get; set; }
        
        public decimal UnitPrice { get; set; }
        
        public int Units { get; set; }
    }
}