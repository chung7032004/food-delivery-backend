using System.Security.Cryptography.X509Certificates;
using FoodDelivery.Common;
using FoodDelivery.DTOs.Order;
using FoodDelivery.Entities;
using FoodDelivery.Repositories;
using FoodDelivery.Repositories.Interfaces;
using FoodDelivery.Service.Interfaces;
namespace FoodDelivery.Service;
public class Shippers1Service : IShippers1Service
{
    private readonly IShippers1Repository _shippers1Repository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository; 
    private readonly FoodContext _context;
    private readonly ILogger<Shippers1Service> _logger;
    private readonly IOrderRepository _orderRepository;
    public Shippers1Service(IShippers1Repository shippers1Repository, IUserRepository userRepository,IRoleRepository roleRepository, FoodContext context, ILogger<Shippers1Service> logger,IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
        _shippers1Repository = shippers1Repository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _context = context;
        _logger = logger;
    }
    public async Task<Result<PagedResponse<ShipperResponse>>> GetAllShippersAsync(int page, int pageSize)
    {   
        pageSize = pageSize > 100 ? 100 : pageSize < 1 ? 10 : pageSize;
        page = page < 1 ? 1 : page;
        var (data, totalCount) = await _shippers1Repository.GetAllAsync(page,pageSize);
        var responseMeta = new PaginationMeta(
            Page : page,
            PageSize : data.Count(),
            TotalCount : totalCount,
            TotalPages : (int)Math.Ceiling((double)totalCount/pageSize)
        );
        var responseData = data.Select(s=> new ShipperResponse
        {
            Id = s.Id,
            UserId = s.UserId,
            IsActive = s.IsActive,
            IsAvailable  = s.IsAvailable, 
            TotalDeliveredOrders  = s.TotalDeliveredOrders,
            FullName = s.User.FullName,
            Email = s.User.Email,
            Phone = s.User.Phone,
            AvatarUrl = s.User.AvatarUrl,
            CreatedAt  = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        }).ToList();
        var response = new PagedResponse<ShipperResponse>(Meta : responseMeta, Data : responseData);
        return Result<PagedResponse<ShipperResponse>>.Success(response);
    }
    public async Task<Result<ShipperResponse>> GetByIdAsync(Guid id)
    {
        var shipper = await _shippers1Repository.GetByIdWithUser(id);
        if(shipper == null)
        {
            return Result<ShipperResponse>.Failure("SHIPPER_NOT_FOUND","Không tìm thất shipper");
        }
        var result = new ShipperResponse
        {
            Id = shipper.Id,
            UserId = shipper.UserId,
            IsActive = shipper.IsActive,
            IsAvailable  = shipper.IsAvailable, 
            TotalDeliveredOrders  = shipper.TotalDeliveredOrders,
            FullName = shipper.User.FullName,
            Email = shipper.User.Email,
            Phone = shipper.User.Phone,
            AvatarUrl = shipper.User.AvatarUrl,
            CreatedAt  = shipper.CreatedAt,
            UpdatedAt = shipper.UpdatedAt
        };
        return Result<ShipperResponse>.Success(result);
    }
    public async Task<Result> RegisterShipperAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if(user == null)
        {
            return Result.Failure("USER_NOT_FOUND","Không tìm thấy tài khoảng người dùng.");
        }
        var existingShipper = await _shippers1Repository.GetByUserIdAsync(userId);
        if (existingShipper != null)
        {
            return Result.Failure("ALREADY_SHIPPER", "Người dùng này đã đăng ký làm shipper rồi.");
        }
        var shipperRole = await _roleRepository.GetByNameAsync("Shipper");            
        if(shipperRole == null)
        {
            return Result.Failure("ROLE_INVALID", "Role Shipper không tồn tại");
        }
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var shipper = new Shipper
            {
                UserId = user.Id,
                IsActive = true,
                IsAvailable = true,
                TotalDeliveredOrders = 0,
                CreatedAt = DateTime.UtcNow,
            };
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = shipperRole.Id,
            };
            await _userRepository.AddUserRoleAsync(userRole);
            await _shippers1Repository.AddAsync(shipper);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return Result.Success();
        }catch(Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đăng ký Shipper cho UserId: {UserId}", userId);
            await transaction.RollbackAsync();
            return Result.Failure("SERVER_ERROR", "Hệ thống đang bận, vui lòng thử lại sau.");
        }
    }
    public async Task<Result> UpdateStatusAsync(Guid shipperId, bool IsActive)
    {
        var shipper = await _shippers1Repository.GetByIdAsync(shipperId);
        if(shipper == null)
        {
            return Result.Failure("SHIPPER_NOT_FOUND","Không tìm thấy shipper");
        }
        shipper.IsActive = IsActive;
        shipper.UpdatedAt = DateTime.UtcNow;
        try
        {
            await _shippers1Repository.UpdateAsync(shipper);
            await _context.SaveChangesAsync();
            return Result.Success();           
        }
        catch(Exception ex)
        {
            _logger.LogError(ex,"Lỗi khi cập nhật trạng thái Shipper {Id}", shipperId);
            return Result.Failure("SERVER_ERROR", "Không thể cập nhật trạng thái lúc này.");
        }
    }
    public async Task<Result<PagedResponse<OrderHistoryItemResponse>>> GetOrderByShipperIdAsync(Guid shipperId, int page, int pageSize)
    {
        pageSize = pageSize > 100 ? 100 : pageSize < 1 ? 10 : pageSize;
        page = page < 1 ? 1 : page;
        var (data, totalCount) = await _orderRepository.GetOrderByShipperIdAsync(shipperId,page,pageSize);
        var responseMeta = new PaginationMeta(
            Page: page,
            PageSize: pageSize,
            TotalCount: totalCount,
            TotalPages: (int)Math.Ceiling((double)totalCount / pageSize)
        );
        var pageResponse = new PagedResponse<OrderHistoryItemResponse>(
            Data: data,
            Meta: responseMeta
        );
        return Result<PagedResponse<OrderHistoryItemResponse>>.Success(pageResponse);
    }
}