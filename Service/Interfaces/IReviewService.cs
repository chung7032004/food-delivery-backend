namespace FoodDelivery.Service.Interfaces
{
    public interface IReviewService
    {
        Task<FoodDelivery.Common.Result> AddReviewAsync(FoodDelivery.DTOs.Review.CreateReviewDto dto, Guid customerId, CancellationToken cancellationToken = default);
    }
}