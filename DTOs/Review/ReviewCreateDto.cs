using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.DTOs.Review
{
    public class ReviewCreateDto
    {
        [Required]
        public Guid OrderItemId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating phải từ 1 đến 5 sao")]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }
    }
}
