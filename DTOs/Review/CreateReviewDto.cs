using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.DTOs.Review
{
    public class CreateReviewDto
    {
        [Required]
        public Guid OrderItemId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }
    }
}
