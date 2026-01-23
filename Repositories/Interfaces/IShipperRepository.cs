using FoodDelivery.Entities;

namespace FoodDelivery.Repositories.Interfaces
{
    public interface IShipperRepository
    {
        // Lấy chi tiết đơn hàng theo ID để kiểm tra trước khi cập nhật
        Task<OrderDetail?> GetOrderDetailByIdAsync(Guid orderId);
        
        // Cập nhật trạng thái đơn hàng (Xác nhận lấy, Thành công, Thất bại)
        void UpdateOrderDetail(OrderDetail orderDetail);
        
        // Lưu lịch sử thay đổi trạng thái (Thầy rất thích hỏi về phần log này)
        Task AddStatusHistoryAsync(OrderStatusHistory history);
        
        // Lưu tất cả thay đổi vào DB
        Task<bool> SaveChangesAsync();
    }
}