using FoodDelivery.Common;
using FoodDelivery.DTOs;
using FoodDelivery.Extensions;
using FoodDelivery.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AddressController : ControllerBase
{
    private readonly IAddressService _addressService;
    public AddressController (IAddressService addressService)
    {
        _addressService = addressService;
    }
    [HttpGet]
    public async Task<IActionResult> GetMyAddress()
    {
        if(!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(Result.Failure("INVALID_TOKEN","Token không hợp lệ hoặc thiếu UserId."));
        }
        var result = await _addressService.GetMyAddresses(userId);
        return Ok(result);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _addressService.GetAddressById(id);
        if (!result.IsSuccess)
        {
            return BadRequest(result); 
        }
        return Ok(result);
    }
    [HttpPost]
    public async Task<IActionResult> Create(AddAddressRequest request)
    {
        if(!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(Result<AddAddressResponse>.Failure("INVALID_TOKEN","Token không hợp lệ hoặc thiếu UserId."));
        }
        var result = await _addressService.AddAddress(userId,request);
        if (result.IsSuccess && result.Data != null)
        {
            return CreatedAtAction(nameof(GetById),new {id = result.Data.Id},result);
        }
        return BadRequest(result);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id , AddressRequest addressRequest)
    {
         if(!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(Result<AddAddressResponse>.Failure("INVALID_TOKEN","Token không hợp lệ hoặc thiếu UserId."));
        }
        var result = await _addressService.UpdateAddress(userId,id, addressRequest);
        if (!result.IsSuccess)
        {
            return NotFound(result);
        }
        return Ok(result);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _addressService.DeleteAddress(id);
        if (!result.IsSuccess)
        {
            return NotFound(result);
        }
        return Ok(result);
    }
    [HttpPatch("{id}/default")]
    public async Task<IActionResult> SetDefault(Guid id)
    {
        if(!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(Result.Failure("INVALID_TOKEN","Token không hợp lệ hoặc thiếu UserId."));
        }
        var result = await _addressService.SetDefault(userId,id);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
    
}