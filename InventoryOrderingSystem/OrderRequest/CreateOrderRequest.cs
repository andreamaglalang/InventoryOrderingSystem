namespace InventoryOrderingSystem.DTOs
{
    public class CreateOrderRequestDto
    {
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}