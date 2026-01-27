using FoodDelivery.Common;
using FoodDelivery.DTOs.Order;

namespace FoodDelivery.Service.Interfaces;
public interface IOrderService
{
    Task<Result<CreateOrderResponseDto>> BuyNowAsync(Guid customerId,BuyNowRequestDto request);
    Task<Result<CreateOrderResponseDto>> CheckoutAsync(Guid customerId,CheckoutRequestDto request);
    Task<Result<ShippingFeeResponseDto>> CalculateShippingFeeAsync(Guid addressId);
    Task<Result<PagedResponse<OrderHistoryItemResponse>>> GetMyOrderAsync(Guid customerId, int page, int pageSize);
    Task<Result<OrderDetailResponse>> GetOrderDetailAsync(Guid userId,List<string> roles,Guid orderId);
    Task<Result> CancelOrderByCustomerAsync(Guid userId, Guid orderId , CancelOrderRequestDto cancelOrderRequestDto);
    Task<Result> RespondToProposalAsync(Guid customerId, Guid orderId, OrderProposalResponseDto request);
    Task<Result<PagedResponse<OrderAdminSummaryResponse>>> GetOrderAdminAsync(OrderFilterModel filter);
    Task<Result> ConfirmOrderAsync(Guid adminId, Guid orderId);
    Task<Result> OutOfStockAsync(Guid adminId, Guid orderId, OutOfStockRequest request);
    Task<Result> StartPreparingAsync(Guid adminId, Guid orderId);
    Task<Result> MarkAsReadyAsync(Guid adminId, Guid orderId);
    Task<Result> CancelOrderByAdminAsync(Guid adminId, Guid orderId, CancelOrderRequestDto request);
}