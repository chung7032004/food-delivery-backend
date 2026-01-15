namespace FoodDelivery.Common;
// Thông tin phân trang
public record PaginationMeta(
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages
);
// Kết quả phân trang
public record PagedResponse<T>(
    IEnumerable<T> Data,
    PaginationMeta Meta
);