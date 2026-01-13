using FoodDelivery.Entities;
using FoodDelivery.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Repositories.Implementations;
public class AddressRepository : IAddressRepository
{
    private readonly FoodContext _context;
    public AddressRepository (FoodContext context)
    {
        _context = context;
    }
    public async Task<List<Address>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Addresses
                .Where(a=>a.UserId == userId)
                .OrderByDescending(a=>a.IsDefault)
                .ToListAsync();
    }
    public async Task <Address?> GetByIdAsync(Guid addressId)
    {
        return await _context.Addresses.FindAsync(addressId);
    }
    public async Task <Address?> GetByIdAndUserIdAsync(Guid addressId, Guid userId)
    {
        return await _context.Addresses
                .FirstOrDefaultAsync(a=>a.Id == addressId && a.UserId == userId);
    }
    public async Task <bool> HasAnyAsync(Guid userId)
    {
        return await _context.Addresses.AnyAsync(a=>a.UserId == userId);
    }
    public async Task <Address?> GetDefaultByUserIdAsync(Guid userId)
    {
        return await _context.Addresses
                .FirstOrDefaultAsync(a=>a.UserId == userId && a.IsDefault) ;
    }
    public async Task AddAsync (Address address)
    {
        await _context.Addresses.AddAsync(address);
    }
    public  Task UpdateAsync (Address address)
    {
        _context.Addresses.Update(address);
        return Task.CompletedTask;
    }
    public  Task DeleteAsync (Address address)
    {
        _context.Addresses.Remove(address);
        return Task.CompletedTask;
    }
}