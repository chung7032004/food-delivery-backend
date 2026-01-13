using FoodDelivery.Common;
using FoodDelivery.DTOs.Account;

namespace FoodDelivery.Service.Interfaces;
public interface IAccountService
{
    Task <Result<AccountResponse>> GetAccountAsync(Guid userId);
    Task <Result> UpdateAccountAsync(Guid id, AccountRequest accountRequest);
}