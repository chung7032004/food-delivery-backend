using FoodDelivery.Entities;

namespace FoodDelivery.Repositories.Interfaces;
public interface IAddressRepository
{
    Task<List<Address>> GetByUserIdAsync(Guid userId);
    Task <Address?> GetByIdAsync(Guid addressId);
    Task <Address?> GetByIdAndUserIdAsync(Guid addressId, Guid userId);
    Task <bool> HasAnyAsync(Guid userId); 
    Task <Address?> GetDefaultByUserIdAsync(Guid userId);
    Task AddAsync (Address address);
    Task UpdateAsync (Address address);
    Task DeleteAsync (Address address);
}