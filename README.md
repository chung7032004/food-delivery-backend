# FoodDelivery Backend API

Backend API cho hệ thống **đặt & giao đồ ăn**, xây dựng bằng **ASP.NET Core Web API**, hỗ trợ xác thực JWT, phân quyền người dùng và làm việc nhóm.

---

## Mục tiêu dự án

- Cung cấp API cho ứng dụng đặt đồ ăn
- Quản lý người dùng, vai trò (Admin / Staff / Customer / Shipper)
- Xác thực & phân quyền bằng JWT + Refresh Token
- Dễ mở rộng, dễ bảo trì, phù hợp làm việc nhóm

---

## 🛠 Công nghệ sử dụng

- **ASP.NET Core 8**
- **Entity Framework Core**
- **PostgreSQL**
- **JWT Bearer Authentication**
- **Swagger / OpenAPI**
- **CORS** (Frontend: React)

---

## Kiến trúc

Áp dụng mô hình **Clean Architecture / Layered Architecture**

### Nguyên tắc:

- Controller: xử lý HTTP, không viết logic nghiệp vụ
- Service: xử lý business logic
- Repository: truy vấn database
- UnitOfWork: quản lý transaction

---

## Cấu trúc thư mục

FoodDelivery
│
├── Controllers // API Controllers
├── Entities // Entity / Domain Models
├── DTOs // Request / Response DTO
│
├── Repositories
│ ├── Interfaces
│ └── Implementations
│
├── Service
│ ├── Interfaces
│ └── Implementations
│
├── Migrations
├── Program.cs
├── appsettings.json
└── README.md

---

## Hướng dẫn chạy dự án

### 1️ Clone source code

```bash
git clone https://github.com/your-username/food-delivery-backend.git
cd food-delivery-backend

### 2️ Cấu hình Database & JWT

{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=FoodDelivery;Username=postgres;Password=123456"
  },
  "TokenSecretKey": "YOUR_SUPER_SECRET_KEY",
}
### 3️ Chạy migration
dotnet ef database update

### 4️ Run project
dotnet run
```
