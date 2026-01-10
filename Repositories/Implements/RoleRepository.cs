using FoodDelivery.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Repositories.Implementations
{
    public class RoleRepository: IRoleRepository
    {
        private readonly FoodContext _context;
        public RoleRepository (FoodContext context)
        {
            _context = context;
        }
        public async Task<Role?> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            return await _context.Roles.FirstOrDefaultAsync(r=>r.Name == name.Trim());
        }
        public async Task<Role?> GetByIdAsync(Guid id)
        {
             if (id == Guid.Empty)
            {
                return null;
            }
            return await _context.Roles.FindAsync(id);
        }
        public async Task AddAsync(Role role)
        {
            await _context.Roles.AddAsync(role); 
        }
    }
}