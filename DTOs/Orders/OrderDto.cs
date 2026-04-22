using AMZN.Data.Entities;

namespace AMZN.DTOs.Orders;

public class OrderDto
{
    public Order? Order { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public ICollection<DeliveryAddress> DeliveryAddresses { get; set; } = new List<DeliveryAddress>();
    public ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();
    
}