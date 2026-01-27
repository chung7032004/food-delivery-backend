using FoodDelivery.Entities;
using FoodDelivery.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Repositories.Implementations;
public class Shipper1Repository : IShippers1Repository{
    
    private readonly FoodContext _context;
    public Shipper1Repository(FoodContext context)
    {
        _context = context;
    }
    public async Task<(List<Shipper> Data , int TotalCount)> GetAllAsync(int page, int pageSize)
    {
        var query =  _context.Shippers
            .AsNoTracking()
            .Include(s=>s.User);
        var toTalCount = await query.CountAsync(); 
        var data = await query        
            .OrderByDescending(s=>s.CreatedAt)
            .Skip((page -1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return(data, toTalCount);
    }
    public async Task<Shipper?> GetByIdAsync(Guid id)
    {
        return await _context.Shippers
            .FindAsync(id);
    }
    public async Task<Shipper?> GetByIdWithUser(Guid id)
    {
        return await _context.Shippers
            .Include(s=>s.User)
            .FirstOrDefaultAsync(s=>s.Id == id);
    }
    public async Task AddAsync(Shipper shipper)
    {
        await _context.Shippers.AddAsync(shipper);
    }
    public async Task UpdateAsync (Shipper shipper)
    {
        _context.Shippers.Update(shipper);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync (Shipper shipper)
    {
        _context.Shippers.Remove(shipper);
    }
}
