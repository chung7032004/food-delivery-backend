using FoodDelivery.Common;
using FoodDelivery.DTOs.Cart;

namespace FoodDelivery.Service.Interfaces;
public interface ICartService
{
    Task<Result<CartResponse>> GetCartAsync (Guid customerId);
    Task<Result> AddCartItemAsync (Guid customerId, CartItemRequest cartItemRequest);
    Task<Result> UpdateQuantityCartItemAsync(Guid customerId,Guid productId,int quantity);
    Task<Result> DeleteCartItemAsync(Guid customerId,Guid productId);
    Task<Result> DeleteCartItemRangeAsync(Guid customerId, IEnumerable<Guid> productIds);
    Task<Result> DeleteCartAsync(Guid customerId);
}