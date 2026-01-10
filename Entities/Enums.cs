public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Preparing = 2,
    Ready = 3,
    OutForDelivery = 4,
    Delivered = 5,
    Cancelled = 6
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
