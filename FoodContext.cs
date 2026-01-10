using FoodDelivery.Entities;
using Microsoft.EntityFrameworkCore;

public class FoodContext : DbContext
{
    public FoodContext(DbContextOptions<FoodContext> options ) : base(options) {}
    public DbSet<Address> Addresses{get;set;}
    public DbSet<Cart> Carts{get;set;}
    public DbSet<CartItem> CartItems{get;set;}
    public DbSet<Category> Categories{get;set;}
    public DbSet<Order> Orders{get;set;}
    public DbSet<OrderDetail> OrderDetails{get;set;}
    public DbSet<OrderItem> OrderItems{get;set;}
    public DbSet<OrderStatusHistory> OrderStatusHistories{get;set;}
    public DbSet<Product> Products{get;set;}
    public DbSet<RefreshToken> RefreshTokens{get;set;}
    public DbSet<Review> Reviews{get;set;}
    public DbSet<Role> Roles{get;set;}
    public DbSet<User> Users{get;set;}
    public DbSet<UserRole> UserRoles{get;set;}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u=>u.Email)
            .IsUnique();
        modelBuilder.Entity<UserRole>()
            .HasKey(ur=>new {ur.UserId, ur.RoleId});
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);
        modelBuilder.Entity<UserRole>()
            .HasOne(ur=>ur.Role)
            .WithMany(r=>r.UserRoles)
            .HasForeignKey(ur=>ur.RoleId);
        modelBuilder.Entity<RefreshToken>()
            .HasOne(rf=>rf.User)
            .WithMany(u=>u.RefreshTokens)
            .HasForeignKey(rt=>rt.UserId);  
        
        modelBuilder.Entity<User>()
            .HasOne(u=>u.Cart)
            .WithOne(c=>c.Customer)
            .HasForeignKey<Cart>(c=>c.CustomerId);

        modelBuilder.Entity<Review>()
            .HasOne(r=>r.Customer)
            .WithMany(u=>u.Reviews)
            .HasForeignKey(r=>r.CustomerId);
        modelBuilder.Entity<Order>()
            .HasOne(o=>o.Review)
            .WithOne(r=>r.Order)
            .HasForeignKey<Review>(r=>r.OrderId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi=>oi.Order)
            .WithMany(o=>o.OrderItems)
            .HasForeignKey(oi=>oi.OrderId);
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi=>oi.Product)
            .WithMany(o=>o.OrderItems)
            .HasForeignKey(oi=>oi.ProductId);

        modelBuilder.Entity<Order>()
            .HasOne(o=>o.Address)
            .WithMany(a=>a.Orders)
            .HasForeignKey(o=>o.AddressId);
        modelBuilder.Entity<Order>()
            .HasOne(o=>o.Customer)
            .WithMany(u=>u.Orders)
            .HasForeignKey(o=>o.CustomerId);
        
        modelBuilder.Entity<Address>()
            .HasOne(a=>a.User)
            .WithMany(u=>u.Addresses)
            .HasForeignKey(a=>a.UserId);

        modelBuilder.Entity<CartItem>()
            .HasIndex(ci=>new {ci.CartId,ci.ProductId})
            .IsUnique(); // 1 loại product không đc xuất hiện trong 1 cart
        modelBuilder.Entity<CartItem>()
            .HasOne(ci=>ci.Cart)
            .WithMany(c=>c.CartItems)
            .HasForeignKey(ci=>ci.CartId);
        modelBuilder.Entity<CartItem>()
            .HasOne(ci=>ci.Product)
            .WithMany(p=>p.CartItems)
            .HasForeignKey(ci=>ci.ProductId);

        modelBuilder.Entity<Product>()
            .HasOne(p=>p.Category)
            .WithMany(c=>c.Products)
            .HasForeignKey(p=>p.CategoryId);
        


        modelBuilder.Entity<OrderDetail>(entity =>
        {
            // 1–1: Order ↔ OrderDetail
            entity.HasOne(od => od.Order)
                .WithOne(o => o.OrderDetail)
                .HasForeignKey<OrderDetail>(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            // Shipper (OrderDetail → User)
            entity.HasOne(od => od.Shipper)
                .WithMany() 
                .HasForeignKey(od => od.ShipperId)
                .OnDelete(DeleteBehavior.Restrict);
            //  Người hủy đơn (OrderDetail → User)
            entity.HasOne(od => od.CancelledByUser)
                .WithMany() 
                .HasForeignKey(od => od.CancelledByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            // Enum → int
            entity.Property(od => od.Status)
                .HasConversion<int>();
            entity.Property(od => od.PaymentMethod)
                .HasConversion<int>();
            entity.Property(od => od.PaymentStatus)
                .HasConversion<int>();
            // Required / Optional
            entity.Property(od => od.CancelReason)
                .HasMaxLength(500);
            entity.Property(od => od.EstimatedDeliveryTime)
                .IsRequired(false);
            entity.Property(od => od.ActualDeliveryTime)
                .IsRequired(false);
        });
    }
}