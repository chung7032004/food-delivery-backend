using FoodDelivery.Common;
using FoodDelivery.DTOs.Order;
using FoodDelivery.Entities;
using FoodDelivery.Repositories.Interfaces;
using FoodDelivery.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Service.Implementations;
public class OrderService :IOrderService
{
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly FoodContext _context;
    public OrderService (
        IProductRepository productRepo,
        IOrderRepository orderRepo,  
        ICartItemRepository cartItemRepo,
        IAddressRepository addressRepo,
        IRestaurantRepository restaurantRepository,
        FoodContext context)
    {
        _productRepository = productRepo;
        _orderRepository = orderRepo;
        _context = context;
        _cartItemRepository = cartItemRepo;
        _addressRepository = addressRepo;
        _restaurantRepository = restaurantRepository;
    }
    public async Task<Result<CreateOrderResponseDto>> BuyNowAsync(Guid customerId,BuyNowRequestDto request)
    {
        if(request.Quantity <= 0)
        {
            return Result<CreateOrderResponseDto>.Failure("INVALID_QUANTITY","Số lượng phải lớn hơn 0");
        }
        var product = await _productRepository.GetByIdAsync(request.ProductId);
        if(product == null)
        {
            return Result<CreateOrderResponseDto>.Failure("PRODUCT_NOT_FOUND","Không tìm thấy sản phẩm.");
        }
        if (!product.IsAvailable)
        {
            return Result<CreateOrderResponseDto>.Failure("PRODUCT_UNAVAILABLE","Đơn hang đang có sản phẩm ngừng bán");
        }
        var statusRestaurant = await _restaurantRepository.GetStatusRestaurantAsync();
        if(statusRestaurant != null &&statusRestaurant.IsOpen == false)
        {
            return Result<CreateOrderResponseDto>.Failure("RESTAURANT_CLOSE",statusRestaurant.ClosingMessage ?? "Quán tạm nghỉ. Bạn có thể quay lại vào hôm sau.");
        }
        var item = new List<CreateOrderItemDto>
        {
            new CreateOrderItemDto()
            {
                ProductId = product.Id,
                ProductImage = product.ImageUrl,
                ProductName = product.Name,
                Quantity  = request.Quantity,
                UnitPrice = product.Price,

            }
        };
        return await CreateOrderInternalAsync(customerId,request.AddressId,item,request.Note);

    }
    public async Task<Result<CreateOrderResponseDto>> CheckoutAsync(Guid customerId,CheckoutRequestDto request)
    {
        var cartItems = await _cartItemRepository.GetByIdsAsync(customerId,request.CartItemIds);
        if (cartItems == null || !cartItems.Any())
        {
            return Result<CreateOrderResponseDto>.Failure("CART_EMPTY","Đơn hàng phải có ít nhất một sản phẩm");
        }
        if (cartItems.Any(ci => ci.Quantity <= 0))
        {
            return Result<CreateOrderResponseDto>.Failure("INVALID_QUANTITY","Số lượng sản phẩm không hợp lệ");
        }
        if (cartItems.Any(ci=>ci.Cart.CustomerId != customerId))
        {
            return Result<CreateOrderResponseDto>.Failure("FORBIDDEN","Không có quyền thanh toán giỏ hàng này");
        }
        if(cartItems.Any(ci=>ci.Product.IsAvailable == false))
        {
            return Result<CreateOrderResponseDto>.Failure("PRODUCT_UNAVAILABLE","Đơn hang đang có sản phẩm ngừng bán");
        }
        var items = cartItems.Select(ci=> new CreateOrderItemDto
        {
                ProductId = ci.Product.Id,
                ProductImage = ci.Product.ImageUrl,
                ProductName = ci.Product.Name,
                Quantity  = ci.Quantity,
                UnitPrice = ci.Product.Price,
        }).ToList();
        var result =  await CreateOrderInternalAsync(customerId,request.AddressId,items,request.Note);
        if (result.IsSuccess)
        {
            await _cartItemRepository.DeleteRangeAsync(cartItems);
            await _context.SaveChangesAsync();
        }
        return result;
        }
    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        Console.WriteLine(lat1);
        Console.WriteLine(lon1);
        Console.WriteLine(lat2);
        Console.WriteLine(lon2);

        var R = 6371; //Bán kính trái đất;
        var dLat = (lat1 - lat2) * (Math.PI/180);
        var dLon = (lon1 - lon2) * (Math.PI/180);
        var a = Math.Sin(dLat/2) * Math.Sin(dLat/2)
            + Math.Cos(lat1* Math.PI/180)*Math.Cos(lat2* Math.PI/180) 
            * Math.Sin(dLon/2)*Math.Sin(dLon/2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a));
        var d = R * c;
        return d *1.2; //Giả sử hệ số đường bộ là 1.2
    }
    private string GenerateOrderCode() => $"FD{DateTime.UtcNow.Ticks}";
    private async Task<Result<CreateOrderResponseDto>> CreateOrderInternalAsync(
        Guid customerId, Guid addressId,
        List<CreateOrderItemDto> items, string? note)
    {
        var address = await _addressRepository.GetByIdAsync(addressId);
        if(address == null)
        {
            return Result<CreateOrderResponseDto>.Failure("ADDRESS_NOT_FOUND","Không tìm thấy địa chỉ");
        }
        var restaurant = await _restaurantRepository.GetMyRestaurant();
        if(restaurant== null)
        {
            return Result<CreateOrderResponseDto>.Failure("RESTAURANT_NOT_CONFIGURED","Cửa hàng chưa được cấu hình.");
        }
        var distance = CalculateDistance(restaurant.Latitude,restaurant.Longitude,address.Latitude,address.Longitude);
        Console.WriteLine($"distance: {distance}");
        if (distance > 40)
        {
            return Result<CreateOrderResponseDto>.Failure("TOO_FAR", "Địa chỉ giao hàng quá xa (tối đa 40km)");
        }
        decimal shippingFee = 15000;
        if(distance > 3)
        {
            shippingFee += (decimal)(distance - 3) * 5000;
        }
        shippingFee = Math.Ceiling(shippingFee/1000)*1000;
        double estimated = 10 + (items.Count -1 ) *2 + (distance * 3);
        DateTime estimatedDeliveryTime = DateTime.UtcNow.AddMinutes(estimated);
        Console.WriteLine("shippingFee",shippingFee);
        var order = new Order()
        {
            CustomerId = customerId,
            Note = note,
            TotalAmount = items.Sum(i=>i.UnitPrice * i.Quantity),
            ShippingFee = shippingFee,
            CreatedAt = DateTime.UtcNow,
            OrderCode = GenerateOrderCode(),
            ReceiverName = address.ReceiverName,
            ShippingAddress = address.FullAddress,
            ReceiverPhone = address.PhoneNumber,
        };
        foreach(var item in items)
        {
            order.OrderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                ProductImage = item.ProductImage,
                ProductName = item.ProductName,
            });
        }
        order.OrderDetail = new OrderDetail
        {
            Status = OrderStatus.Pending,
            PaymentMethod = PaymentMethod.Cash,
            PaymentStatus = PaymentStatus.Unpaid,
            EstimatedDeliveryTime = estimatedDeliveryTime
        };
        order.OrderStatusHistories.Add(new OrderStatusHistory()
        {
          OrderId = order.Id,
          ChangeByUserId = customerId,
          Status = OrderStatus.Pending,
          ChangedAt = DateTime.UtcNow,
          Note = "Bạn đã tạo đơn hàng."

        });
        await _orderRepository.AddAsync(order);
        await _context.SaveChangesAsync();
        return Result<CreateOrderResponseDto>.Success(new CreateOrderResponseDto
        {
            OrderCode = order.OrderCode,
            OrderId = order.Id,
            Status = order.OrderDetail.Status,
            TotalAmount = order.TotalAmount,
            ShippingFee = shippingFee,
            EstimatedDeliveryTime = estimatedDeliveryTime
        });
    }
    public async Task<Result<PagedResponse<OrderHistoryItemResponse>>> GetMyOrderAsync(Guid customerId, int page, int pageSize)
    {
        pageSize = pageSize > 100 ? 100 : pageSize < 1 ? 10 : pageSize;
        page = page < 1 ? 1 : page;
        var order = await _orderRepository.GetMyOrderAsync(customerId,page,pageSize);
        var totalCount = await _orderRepository.GetTotalCountAsync(customerId);
        var responseMeta = new PaginationMeta(
            Page: page,
            PageSize: pageSize,
            TotalCount: totalCount,
            TotalPages: (int)Math.Ceiling((double)totalCount / pageSize)
        );
        var pageResponse = new PagedResponse<OrderHistoryItemResponse>(
            Data: order,
            Meta: responseMeta
        );
        return Result<PagedResponse<OrderHistoryItemResponse>>.Success(pageResponse);
    }
    public async Task<Result<OrderDetailResponse>> GetOrderDetailAsync(Guid userId, List<string> roles, Guid orderId)
    {   
        
        var order = await _orderRepository.GetOrderDetailAsync(orderId);
        if(order == null)
        {
            return Result<OrderDetailResponse>.Failure("ORDER__NOT_FOUND","Không tìm thấy đơn hàng này");
        }
        bool isAdmin = roles.Contains("Admin");
        bool isOwner = order.CustomerId == userId;
        if (!isAdmin && !isOwner)
        {
            return Result<OrderDetailResponse>.Failure("ORDER__FORBIDDEN", "Bạn không có quyền xem đơn hàng này");
        }
        var response = new OrderDetailResponse
        {
            CustomerId = order.CustomerId,
            CustomerEmail = order.Customer.Email,
            CustomerName = order.Customer.FullName,
            OrderId = order.Id,
            OrderCode = order.OrderCode,
            CreatedAt = order.CreatedAt,
            ShippingAddress  = order.ShippingAddress,
            ReceiverName  = order.ReceiverName,
            ReceiverPhone  = order.ReceiverPhone,
            ShippingFee = order.ShippingFee, 
            EstimatedDeliveryTime  = order.OrderDetail.EstimatedDeliveryTime,
            ActualDeliveryTime = order.OrderDetail.ActualDeliveryTime,
            PaymentMethod = order.OrderDetail.PaymentMethod,
            PaymentStatus = order.OrderDetail.PaymentStatus,
            CurrentStatus =order.OrderDetail.Status,
            CancelReason = order.OrderDetail.CancelReason,
            TotalAmount = order.TotalAmount,
        };
        foreach(var item in order.OrderItems)
        {
            response.Items.Add(new OrderItemResponse
            {
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            ProductImage  = item.ProductImage,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TotalPrice = item.UnitPrice * item.Quantity,
            IsRemoved = item.IsRemoved,
            RemoveReason = item.RemoveReason,
        });
        }
        foreach(var history in order.OrderStatusHistories)
        {
            response.StatusHistories.Add(new OrderStatusHistoryResponse
            {
                Status = history.Status,
                ChangedAt = history.ChangedAt,
                ChangedBy  = history.ChangeByUser != null
                    ? history.ChangeByUser.FullName
                    : "System",
                Note  = history.Note,
            });
        }
        return Result<OrderDetailResponse>.Success(response);
    }   
    public async Task<Result> CancelOrderByCustomerAsync(Guid userId,Guid orderId, CancelOrderRequestDto request)
    {
        var order = await _orderRepository.GetOrderById(orderId);
        if(order == null)
        {
            return Result.Failure("ORDER_NOT_FOUND","Không tìm thấy đơn hàng.");
        }
        if (order.OrderDetail.Status != OrderStatus.Pending && 
            order.OrderDetail.Status != OrderStatus.Confirmed &&
            order.OrderDetail.Status != OrderStatus.WaitingCustomerDecision)
        {
            return Result.Failure("ORDER_CANNOT_CANCEL", "Đơn hàng đã được chế biến hoặc đang giao, không thể hủy.");
        }
        if (order.CustomerId != userId)
        {
            return Result.Failure("ORDER_FORBIDDEN","Bạn không có quyền huỷ đơn hàng này");
        }

        var orderStatusHistory = new OrderStatusHistory
        {
            Status = OrderStatus.Cancelled,
            OrderId = orderId,
            ChangeByUserId = userId,
            ChangedAt = DateTime.UtcNow,
            Note = request.Reason,
            ActionBy = OrderActionBy.Customer
        };
        order.OrderDetail.Status = OrderStatus.Cancelled;
        order.OrderDetail.CancelledByUserId = userId;
        order.OrderDetail.CancelReason = request.Reason;
        await _orderRepository.AddOrderHistoryAsync(orderStatusHistory);
        await _context.SaveChangesAsync();
        // Nếu khách đã thanh toán qua ví/thẻ -> Gọi Service hoàn tiền.
        return Result.Success();
    }
    public async Task<Result> RespondToProposalAsync(Guid customerId, Guid orderId, OrderProposalResponseDto request)
    {
        var order = await _orderRepository.GetOrderById(orderId);
        if(order == null || order.CustomerId != customerId)
        {
            return Result.Failure("ORDER_NOT_FOUND","Không tìm thấy đơn hàng");
        }
        if(order.OrderDetail.Status != OrderStatus.WaitingCustomerDecision)
        {
            return Result.Failure("INVALID_STATUS","Đơn hàng không ở trạng thái chờ xác nhận");
        }
        // bỏ món
        var orderStatusHistory = new OrderStatusHistory
        {
            OrderId = orderId,
            // ChangeByUserId = customerId,
            ChangedAt = DateTime.UtcNow,
            Note = request.Note ?? string.Empty,
        };
        if (request.IsAccepted)
        {
            var itemsToRemove = order.OrderItems.Where(oi=> !oi.IsRemoved).ToList();
            if (!itemsToRemove.Any()) {
                return Result.Failure("DATA_ERROR", "Không tìm thấy món hàng nào cần loại bỏ.");
            }
            foreach(var item in itemsToRemove)
            {
                order.OrderItems.Remove(item);
            }
            if (!order.OrderItems.Any())
            {
                order.OrderDetail.Status = OrderStatus.Cancelled;
                orderStatusHistory.Status = OrderStatus.Cancelled;
                orderStatusHistory.ActionBy = OrderActionBy.System;
                orderStatusHistory.Note = "Hủy tự động do không còn món nào khả dụng.";
                await _orderRepository.AddOrderHistoryAsync(orderStatusHistory);
                await _context.SaveChangesAsync();
                return Result.Failure("EMPTY_ORDER","Đơn hàng đã bị hủy vì không còn món nào.");
            }
            orderStatusHistory.Status = OrderStatus.Confirmed;
            order.OrderDetail.Status = OrderStatus.Confirmed;
            orderStatusHistory.ActionBy = OrderActionBy.Customer;
            order.TotalAmount = order.OrderItems.Sum(oi=> oi.Quantity*oi.UnitPrice);
        }
        else
        {
            order.OrderDetail.Status = OrderStatus.Cancelled;
            orderStatusHistory.Status = OrderStatus.Cancelled;
            orderStatusHistory.ActionBy = OrderActionBy.Customer;
            orderStatusHistory.Note = request.Note ?? "Khách hàng không đồng ý với thay đổi từ nhà hàng.";
        }
        await _orderRepository.AddOrderHistoryAsync(orderStatusHistory);
        await _context.SaveChangesAsync();
        return Result.Success();
    }
    public async Task<Result<PagedResponse<OrderAdminSummaryResponse>>> GetOrderAdminAsync(OrderFilterModel filter)
    {
        var query = _orderRepository.GetQueryable();
        if (filter.Status.HasValue)
        {
            query = query.Where(o=>o.OrderDetail.Status == filter.Status.Value);
        }
        if (!string.IsNullOrWhiteSpace(filter.SearchCode))
        {
            string search = filter.SearchCode.Trim().ToLower();
            query = query.Where(o=>o.OrderCode.ToLower().Contains(search) 
                || o.Customer.FullName.ToLower().Contains(search) );
        }
        if (filter.FromDate.HasValue)
        {
            query = query.Where(o=>o.CreatedAt >= filter.FromDate.Value);
        }
        if (filter.ToDate.HasValue)
        {
            var EndOfDay = filter.ToDate.Value.AddDays(1).AddTicks(-1);
            query = query.Where(o=>o.CreatedAt <= EndOfDay);
        }
        int totalCount = await query.CountAsync();
        var pageSize = filter.PageSize > 100 ? 100 : filter.PageSize < 1 ? 10 : filter.PageSize;
        var page = filter.Page < 1 ? 1 : filter.Page;
        var items = await query
            .OrderByDescending(o=>o.CreatedAt)
            .Skip((page -1 ) *pageSize)
            .Take(pageSize)
            .Select(o=> new OrderAdminSummaryResponse
            {
                Id = o.Id,
                OrderCode = o.OrderCode,
                CustomerName = o.Customer.FullName,
                TotalAmount = o.TotalAmount,
                Status = o.OrderDetail.Status,
                CreatedAt = o.CreatedAt,
                PaymentMethod = o.OrderDetail.PaymentMethod
            }).ToListAsync();
        var responseMeta = new PaginationMeta(
            Page: page,
            PageSize: pageSize,
            TotalCount: totalCount,
            TotalPages: (int)Math.Ceiling((double)totalCount / pageSize)
        );   
        var pageResponse = new PagedResponse<OrderAdminSummaryResponse>(
            Data: items,
            Meta: responseMeta
        );  
        return Result<PagedResponse<OrderAdminSummaryResponse>>.Success(pageResponse);
    }
    public async Task<Result> ConfirmOrderAsync(Guid adminId, Guid orderId)
    {
        var order = await _orderRepository.GetOrderById(orderId);
        if(order == null)
        {
            return Result.Failure("ORDER_NOT_FOUND","Không tìm thấy đơn hàng.");
        }
        if(order.OrderDetail.Status != OrderStatus.Pending)
        {
            return Result.Failure("INVALID_STATUS",$"Không thể xác nhận đơn hàng ở trạng thái {order.OrderDetail.Status}");
        }
        order.OrderDetail.Status = OrderStatus.Confirmed;
        var history = new OrderStatusHistory
        {
            ActionBy = OrderActionBy.Admin,
            ChangeByUserId = adminId,
            ChangedAt = DateTime.UtcNow,
            Note = "Admin đã xác nhận đơn hàng.",
            OrderId = orderId,
            Status = OrderStatus.Confirmed
        };

        await _orderRepository.AddOrderHistoryAsync(history);
        await _context.SaveChangesAsync();
        return Result.Success();
    }
    public async Task<Result> OutOfStockAsync(Guid adminId, Guid orderId, OutOfStockRequest request)
    {
        var order = await _orderRepository.GetOrderById(orderId);
        if(order == null)
        { 
            return Result.Failure("ORDER_NOT_FOUND","Không tìm thấy đơn hàng.");
        }
        if(order.OrderDetail.Status != OrderStatus.Pending)
        {
            return Result.Failure("INVALID_STATUS",$"Không thể xác nhận đơn hàng ở trạng thái {order.OrderDetail.Status}");
        }
        order.OrderDetail.Status = OrderStatus.WaitingCustomerDecision;
        var itemsToUpdate = order.OrderItems.Where(oi=> request.RemovedProductIds.Contains(oi.ProductId)).ToList();
        if (!itemsToUpdate.Any())
        {
            return Result.Failure("NO_ITEMS_FOUND", "Không tìm thấy món ăn nào khớp với danh sách yêu cầu.");
        }
        foreach (var item in itemsToUpdate)
        {
            item.IsRemoved = true;
            item.RemoveReason = "Món ăn thiếu nguyên liệu để làm.";
        }
        var history = new OrderStatusHistory
        {
            ActionBy = OrderActionBy.Admin,
            ChangeByUserId = adminId,
            ChangedAt = DateTime.UtcNow,
            Note = "Một số món ăn đã hết hàng, chờ khách hàng quyết định.",
            OrderId = orderId,
            Status = OrderStatus.WaitingCustomerDecision
        };
        await _orderRepository.AddOrderHistoryAsync(history);
        await _context.SaveChangesAsync();
        return Result.Success(); 
    }
    //chuyển trạng thái qua preparing
    public async Task<Result> StartPreparingAsync(Guid adminId, Guid orderId)
    {
        var order = await _orderRepository.GetOrderById(orderId);
        if (order == null)
        {
            return Result.Failure("ORDER_NOT_FOUND", "Không tìm thấy đơn hàng.");
        }
        if (order.OrderDetail.Status != OrderStatus.Confirmed)
        {
            return Result.Failure("INVALID_STATUS", $"Không thể nấu đơn hàng ở trạng thái {order.OrderDetail.Status}");
        }
        order.OrderDetail.Status = OrderStatus.Preparing;
        var history = new OrderStatusHistory
        {
            ActionBy = OrderActionBy.Admin,
            ChangeByUserId = adminId,
            ChangedAt = DateTime.UtcNow,
            Note = "Nhà hàng bắt đầu chế biến món ăn.",
            OrderId = orderId,
            Status = OrderStatus.Preparing
        };
        await _orderRepository.AddOrderHistoryAsync(history);
        await _context.SaveChangesAsync();
        //Gửi Notification cho Customer: "Món ăn của bạn đang được chế biến"
        //Kích hoạt hệ thống tìm Shipper
        return Result.Success();
    }
    //Nấu xong và sẵn sang cho shipper lấy hàng
    public async Task<Result> MarkAsReadyAsync(Guid adminId, Guid orderId)
    {
        var order = await _orderRepository.GetOrderById(orderId);
        if (order == null)
        {
            return Result.Failure("ORDER_NOT_FOUND", "Không tìm thấy đơn hàng.");
        }
        if (order.OrderDetail.Status != OrderStatus.Preparing)
        {
            return Result.Failure("INVALID_STATUS", "Đơn hàng chưa được nấu xong hoặc đang ở trạng thái khác.");
        }
        order.OrderDetail.Status = OrderStatus.ReadyForPickup;
        var history = new OrderStatusHistory
        {
            ActionBy = OrderActionBy.Admin,
            ChangeByUserId = adminId,
            ChangedAt = DateTime.UtcNow,
            Note = "Món ăn đã sẵn sàng. Chờ Shipper đến lấy.",
            OrderId = orderId,
            Status = OrderStatus.ReadyForPickup
        };
        await _orderRepository.AddOrderHistoryAsync(history);
        await _context.SaveChangesAsync();
        // TODO: Gửi Notification cho Shipper: "Đơn hàng #... đã sẵn sàng, mời bạn vào lấy món."
        return Result.Success();
    }
    public async Task<Result> CancelOrderByAdminAsync(Guid adminId, Guid orderId, CancelOrderRequestDto request)
    {   
        var order = await _orderRepository.GetOrderById(orderId);
        if (order == null)
        {
            return Result.Failure("ORDER_NOT_FOUND", "Không tìm thấy đơn hàng.");
        }
        if (order.OrderDetail.Status == OrderStatus.Completed || order.OrderDetail.Status == OrderStatus.Cancelled)
        {
            return Result.Failure("INVALID_STATUS", $"Không thể hủy đơn hàng đã {order.OrderDetail.Status}.");
        }
        var orderStatusHistory = new OrderStatusHistory
        {
            Status = OrderStatus.Cancelled,
            OrderId = orderId,
            ChangeByUserId = adminId,
            ChangedAt = DateTime.UtcNow,
            Note = request.Reason,
            ActionBy = OrderActionBy.Admin 
        };

        order.OrderDetail.Status = OrderStatus.Cancelled;
        order.OrderDetail.CancelledByUserId = adminId;
        order.OrderDetail.CancelReason = request.Reason;
        await _orderRepository.AddOrderHistoryAsync(orderStatusHistory);
        await _context.SaveChangesAsync();
        // Nếu khách đã thanh toán qua ví/thẻ -> Gọi Service hoàn tiền.
        // Gửi thông báo Push cho Khách hàng: "Đơn hàng của bạn đã bị hủy bởi nhà hàng".
        // Gửi thông báo cho Shipper (nếu đã có shipper nhận đơn): "Đơn hàng đã bị hủy, bạn không cần đến quán".
        return Result.Success();
    }            
    //tiệm đóng cửa
}