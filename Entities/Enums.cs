public enum OrderStatus
{
    Pending = 1,      // Vừa tạo
    Confirmed = 2,    // Shop xác nhận
    Shipping = 3,     // Đang giao
    Completed = 4,    // Giao thành công
    Cancelled = 5     // Bị huỷ
}

public enum PaymentMethod
{
    Cash = 0,
    Card = 1,
    Momo = 2
}
public enum PaymentStatus
{
    Unpaid = 0,
    Paid = 1,
    Refunded = 2
}
