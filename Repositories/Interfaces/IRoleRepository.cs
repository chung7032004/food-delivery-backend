using FoodDelivery.Entities;

namespace FoodDelivery.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(string name);
        Task<Role?> GetByIdAsync(Guid id);
        Task AddAsync(Role role);
    }
}