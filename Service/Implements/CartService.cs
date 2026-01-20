using FoodDelivery.Common;
using FoodDelivery.DTOs.Cart;
using FoodDelivery.Entities;
using FoodDelivery.Repositories.Interfaces;
using FoodDelivery.Service.Interfaces;

namespace FoodDelivery.Service.Implementations;
public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IProductRepository _productRepository;
    private readonly FoodContext _context;

    public CartService (ICartRepository cartRepository, ICartItemRepository cartItemRepository ,IProductRepository productRepository, FoodContext context)
    {
        _cartRepository = cartRepository;
        _cartItemRepository = cartItemRepository;
        _context = context;
        _productRepository = productRepository;
    } 
    public async Task<Result<CartResponse>> GetCartAsync (Guid customerId)
    {
        var cart = await _cartRepository.GetCartWithItemsByCustomerIdAsync(customerId);
        if(cart == null)
        {
            return Result<CartResponse>.Failure("CART_NOT_FOUND","Không tìm thấy giỏ hàng");
        }
        var response = new CartResponse
        {
            CustomerId = cart.CustomerId,
            CreatedAt = cart.CreatedAt,
            UpdatedAt =cart.UpdatedAt,
            CartItems = cart.CartItems.Select(ci=> new CartItemDto
            {
                Id = ci.Id,
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                CategoryId = ci.Product.CategoryId,
                CategoryName = ci.Product.Category.Name,
                Name = ci.Product.Name,
                Price = ci.Product.Price,
                ImageUrl = ci.Product.ImageUrl,
                AddedAt = ci.AddedAt, 
                UpdatedAt = ci.UpdatedAt,
            }).ToList()
        };
        return Result<CartResponse>.Success(response);
    }
    public async Task<Result> AddCartItemAsync (Guid customerId, CartItemRequest cartItemRequest)
    {   
        if(cartItemRequest.Quantity <= 0)
        {
            return Result.Failure("INVALID_QUANTITY","Số lượng phải lớn hơn 0");
        }
        var product = await _productRepository.GetByIdAsync(cartItemRequest.ProductId);

        if (product == null)
        {
            return Result.Failure("PRODUCT_NOT_FOUND", "Sản phẩm không tồn tại");
        }

        if (!product.IsAvailable)
        {
            return Result.Failure("PRODUCT_UNAVAILABLE", "Món ăn hiện không khả dụng");
        }

        var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
        if(cart == null)
        {
            cart = new Cart
            {
                CustomerId = customerId,
                CreatedAt = DateTime.UtcNow
            };
            await _cartRepository.AddAsync(cart);
        }
        var cartItem = await _cartItemRepository.GetByCartIdAndProductIdAsync(cart.Id, cartItemRequest.ProductId);
        if(cartItem != null)
        {
            cartItem.Quantity +=cartItemRequest.Quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;
            await _cartItemRepository.UpdateAsync(cartItem);
        }
        else
        {
            cartItem = new CartItem
            {
                AddedAt = DateTime.UtcNow,
                CartId = cart.Id,
                ProductId = cartItemRequest.ProductId,
                Quantity = cartItemRequest.Quantity,    
            };
            await _cartItemRepository.AddAsync(cartItem);
        }
        
        await _context.SaveChangesAsync();
        return Result.Success();
    }
    public async Task<Result> UpdateQuantityCartItemAsync(Guid customerId,Guid productId,int quantity)
    {
        if(quantity <= 0)
        {
            return Result.Failure("INVALID_QUANTITY","Số lượng phải lớn hơn 0");
        }

        var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
        if(cart == null)
        {
            return Result.Failure("CART_NOT_FOUND","Không tìm thấy giỏ hàng");
        }

        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
        {
            return Result.Failure("PRODUCT_NOT_FOUND", "Sản phẩm không tồn tại");
        }
        if (!product.IsAvailable)
        {
            return Result.Failure("PRODUCT_UNAVAILABLE", "Món ăn hiện không khả dụng");
        }   

        var cartItem = await _cartItemRepository.GetByCartIdAndProductIdAsync(cart.Id,productId);
        if(cartItem == null)
        {
            return Result.Failure("CART_ITEM_NOT_FOUND","Món ăn không tồn tại trong giỏ hàng");
        }
        
        cartItem.Quantity = quantity;
        cartItem.UpdatedAt = DateTime.UtcNow;
        await _cartItemRepository.UpdateAsync(cartItem);
        await _context.SaveChangesAsync();
        return Result.Success();
    }
    public async Task<Result> DeleteCartItemAsync(Guid customerId,Guid productId)
    {
        var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
        if(cart == null)
        {
            return Result.Failure("CART_NOT_FOUND","Không tìm thấy giỏ hàng");
        }

        var cartItem = await _cartItemRepository.GetByCartIdAndProductIdAsync(cart.Id,productId);
        if(cartItem == null)
        {
            return Result.Failure("CART_ITEM_NOT_FOUND","Món ăn không tồn tại trong giỏ hàng");
        }
        await _cartItemRepository.DeleteAsync(cartItem);
        await _context.SaveChangesAsync();
        return Result.Success();
    }
    public async Task<Result> DeleteCartItemRangeAsync(Guid customerId,IEnumerable<Guid> productIds)
    {
        if (productIds == null || !productIds.Any())
        {
            return Result.Failure(
                "EMPTY_PRODUCT_IDS",
                "Danh sách sản phẩm cần xóa trống"
            );
        }

        var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
        if (cart == null)
        {
            return Result.Failure(
                "CART_NOT_FOUND",
                "Không tìm thấy giỏ hàng"
            );
        }

        var cartItems = await _cartItemRepository.GetByCartIdAsync(cart.Id);
        var itemsToDelete = cartItems
            .Where(ci => productIds.Contains(ci.ProductId))
            .ToList();

        if (!itemsToDelete.Any())
        {
            return Result.Failure(
                "CART_ITEM_NOT_FOUND",
                "Không có món nào hợp lệ để xóa"
            );
        }
        await _cartItemRepository.DeleteRangeAsync(itemsToDelete);
        await _context.SaveChangesAsync();
        return Result.Success();
    }
    public async Task<Result> DeleteCartAsync(Guid customerId)
    {
        var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
        if (cart == null)
        {
            return Result.Failure(
                "CART_NOT_FOUND",
                "Không tìm thấy giỏ hàng"
            );
        }

        var cartItems = await _cartItemRepository.GetByCartIdAsync(cart.Id);
        if (!cartItems.Any())
        {
           return Result.Success(); 
        }
        await _cartItemRepository.DeleteRangeAsync(cartItems);
        await _context.SaveChangesAsync();
        return Result.Success();
    }

}