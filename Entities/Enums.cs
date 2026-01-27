public enum OrderStatus
{
    Pending = 1,                    // Khách vừa đặt
    WaitingCustomerDecision = 2,    // Nhà hàng báo hết món → chờ khách phản hồi
    Confirmed = 3,                  // Khách đồng ý tiếp tục
    Preparing = 4,                  // Đang nấu
    ReadyForPickup = 5,             // Chờ shipper
    Shipping = 6,                   // Đang giao
    Completed = 7,                  // Hoàn tất
    Cancelled = 8                   // Bị hủy
}
public enum OrderActionBy
{
    Customer = 1,
    Shipper = 2,
    Admin = 3,
    System = 4,
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
