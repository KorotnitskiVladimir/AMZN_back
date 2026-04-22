using AMZN.Data.Entities;

namespace AMZN.DTOs.Orders;

public class CompletedOrderDto
{
    public Order Order { get; set; } = null!;
    public int TotalQuantity { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public DeliveryAddress? DeliveryAddress { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
}