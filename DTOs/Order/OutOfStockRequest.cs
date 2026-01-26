namespace FoodDelivery.DTOs.Order
{
    public class OutOfStockRequest
    {
        public List<Guid> RemovedProductIds { get; set; } = new List<Guid>();
        public string Note { get; set; } = string.Empty;
    }
}