using FoodDelivery.Repositories.Interfaces;

namespace FoodDelivery.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FoodContext _context;
        public UnitOfWork (FoodContext context)
        {
            _context = context;
        }
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }    
}
