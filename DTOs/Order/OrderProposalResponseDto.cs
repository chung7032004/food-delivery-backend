namespace FoodDelivery.DTOs.Order;
public class OrderProposalResponseDto
{
    public bool IsAccepted { get; set; }
    public string? Note { get; set; } // Lý do nếu từ chối hoặc lời nhắn nếu đồng ý
}