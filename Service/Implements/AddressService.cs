using System.Runtime.CompilerServices;
using FoodDelivery.Common;
using FoodDelivery.DTOs;
using FoodDelivery.Entities;
using FoodDelivery.Repositories.Interfaces;
using FoodDelivery.Service.Interfaces;

namespace FoodDelivery.Service.Implementations;
public class AddressService : IAddressService
{
    private readonly IAddressRepository _addressRepository;
    private readonly IUnitOfWork _unitOfWork;
    public AddressService (IAddressRepository addressRepository, IUnitOfWork unitOfWork)
    {
        _addressRepository = addressRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task <Result<List<AddressResponse>>> GetMyAddresses(Guid userId)
    {
        var addresses = await _addressRepository.GetByUserIdAsync(userId);
        var addressesResponse = addresses.Select(address => new AddressResponse
        {
            Id = address.Id,
            FullAddress = address.FullAddress,
            IsDefault = address.IsDefault,
            Label = address.Label,
            Latitude = address.Latitude,    
            Longitude = address.Longitude,
        }).ToList();
        return Result<List<AddressResponse>>.Success(addressesResponse);
    }
    public async Task <Result<AddressResponse>> GetAddressById(Guid addressId)
    {
        var address = await _addressRepository.GetByIdAsync(addressId);
        if(address == null)
        {
            return Result<AddressResponse>.Failure("ADDRESS_NOT_FOUND","Không tìm thấy địa chỉ");
        }
        var response = new AddressResponse
        {
            FullAddress = address.FullAddress,
            Id = address.Id,
            IsDefault = address.IsDefault,
            Label = address.Label,
            Latitude = address.Latitude,
            Longitude = address.Longitude,
        };
        return Result<AddressResponse>.Success(response);
    }
    public async Task <Result<AddAddressResponse>> AddAddress (Guid userId, AddAddressRequest addressRequest)
    {
        if(addressRequest == null)
        {
            return Result<AddAddressResponse>.Failure("ADDRESS_INVALID","Địa chỉ thêm vào không hợp lệ");
        }
        var address = new Address
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            IsDefault = false,
            FullAddress = addressRequest.FullAddress,
            Label = addressRequest.Label,
            Latitude = addressRequest.Latitude,
            Longitude = addressRequest.Longitude,
            CreatedAt = DateTime.UtcNow
        };
        var hasAny = await _addressRepository.HasAnyAsync(userId);
        if (!hasAny)
        {
            address.IsDefault = true;
        }
        else if (addressRequest.IsDefault)
        {
            var addressDefault = await _addressRepository.GetDefaultByUserIdAsync(userId);
            if(addressDefault != null)
            {
                addressDefault.IsDefault = false;
            }
            address.IsDefault = true;
        }
        await _addressRepository.AddAsync(address);
        await _unitOfWork.SaveChangesAsync();
        var response = new AddAddressResponse
        {
            Id = address.Id,
            FullAddress = address.FullAddress,
            Label = address.Label,
            IsDefault = address.IsDefault,
            Latitude = address.Latitude,
            Longitude = address.Longitude,
            CreateAt = address.CreatedAt
        };

        return Result<AddAddressResponse>.Success(response);
    }
    public async Task <Result> UpdateAddress (Guid userId, Guid addressId, AddressRequest addressRequest)
    {
        var address = await _addressRepository.GetByIdAsync(addressId);
        if(address == null)
        {
            return Result.Failure("ADDRESS_NOT_FOUND","Không tìm thấy địa chỉ");
        }
        address.FullAddress = addressRequest.FullAddress;
        address.Label = addressRequest.Label;
        address.Latitude = addressRequest.Latitude;
        address.Longitude = addressRequest.Longitude;
        address.UpdatedAt = DateTime.UtcNow;
        if (addressRequest.IsDefault)
        {
            var addressDefault = await _addressRepository.GetDefaultByUserIdAsync(userId);
            if(addressDefault != null && address.Id != addressDefault.Id)
            {
                addressDefault.IsDefault = false;
                await _addressRepository.UpdateAsync(addressDefault);
                await _unitOfWork.SaveChangesAsync();
            }
            address.IsDefault = true;
        }
        await _addressRepository.UpdateAsync(address);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
    public async Task <Result> DeleteAddress (Guid addressId)
    {
        var address = await _addressRepository.GetByIdAsync(addressId);
        if(address == null)
        {
            return Result.Failure("ADDRESS_NOT_FOUND","Không tìm thấy địa chỉ");
        }
        bool wasDefault = address.IsDefault;
        Guid userId = address.UserId;
        await _addressRepository.DeleteAsync(address);

        if (wasDefault)
        {
            await _unitOfWork.SaveChangesAsync();
            var remainingAddress = await _addressRepository.GetByUserIdAsync(userId);
            var newAddress = remainingAddress.OrderByDescending(r=>r.CreatedAt).FirstOrDefault();
            if(newAddress!= null)
            {
                newAddress.IsDefault = true;
                await _addressRepository.UpdateAsync(newAddress);
            }
        }
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
    public async Task <Result> SetDefault (Guid userId, Guid addressId)
    {
        var address = await _addressRepository.GetByIdAndUserIdAsync(addressId,userId);
        if(address == null)
        {
            return Result.Failure("ADDRESS_NOT_FOUND","Không tìm thấy địa chỉ");
        }
        var addressDefault = await _addressRepository.GetDefaultByUserIdAsync(userId);
        if (addressDefault == null || address.Id != addressDefault.Id)
        {
            if(addressDefault != null)
            {
                addressDefault.IsDefault = false;
                await _addressRepository.UpdateAsync(addressDefault);
                await _unitOfWork.SaveChangesAsync();
            }
            address.IsDefault = true;
        }
        await _addressRepository.UpdateAsync(address);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}