using FoodDelivery.Common;
using FoodDelivery.Repositories.Interfaces;
using FoodDelivery.Service.Interfaces;
namespace FoodDelivery.Service;
public class Shippers1Service : IShippers1Service
{
    private readonly IShippers1Repository _shippers1Repository;
    public Shippers1Service(IShippers1Repository shippers1Repository)
    {
        _shippers1Repository = shippers1Repository;
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
    
}