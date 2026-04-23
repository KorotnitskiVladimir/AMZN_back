using AMZN.Data.Entities;
using AMZN.DTOs.Orders;
using AMZN.Repositories.Carts;
using AMZN.Repositories.Orders;
using AMZN.Repositories.Products;
using AMZN.Repositories.Users;
using AMZN.Shared.Exceptions;
using AMZN.Shared.Exceptions.Errors;

namespace AMZN.Services.Orders;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;

    public OrderService(IOrderRepository orderRepository,
        ICartRepository cartRepository,
        IProductRepository productRepository,
        IUserRepository userRepository)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _userRepository = userRepository;
    }

    public async Task<OrderDto> CreateOrderAsync(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new ApiException(ErrorCodes.UserNotFound, "User not found", StatusCodes.Status404NotFound);
        }

        var addresses = await _userRepository.GetUserDeliveryAddressesAsync(user);
        if (addresses == null)
        {
            throw new ApiException(ErrorCodes.UserNotFound, "User not found", StatusCodes.Status404NotFound);
        }
        
        var paymentMethods = await _userRepository.GetUserPaymentMethodsAsync(user);
        if (paymentMethods == null)
        {
            throw new ApiException(ErrorCodes.UserNotFound, "User not found", StatusCodes.Status404NotFound);
        }
        
        var cart = await ValidateCartAsync(userId);

        var order = await _orderRepository.GetPendingOrderByUserIdAsync(user.Id);
        if (order != null)
        {
            throw new ApiException(ErrorCodes.PendingOrderExists, "Pending order exists", StatusCodes.Status409Conflict);
        }
        
        int quantity = 0;
        decimal totalPrice = 0;
        foreach (var ci in cart.Items)
        {
            var product = await _productRepository.GetByIdAsync(ci.ProductId);
            if (product == null)
            {
                throw new ApiException(ErrorCodes.ProductNotFound, "Product not found", StatusCodes.Status404NotFound);
            }
            
            quantity += ci.Quantity;
            totalPrice += product.CurrentPrice * ci.Quantity;
        }

        if (quantity == 0)
        {
            throw new ApiException(ErrorCodes.CartNotFound, "Cart is empty", StatusCodes.Status404NotFound);
        }

        order = new()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TotalAmount = totalPrice,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.Now,
            User = user,
            OrderItems = new List<OrderItem>()
        };
        await _orderRepository.AddOrderAsync(order);
        foreach (var ci in cart.Items)
        {
            OrderItem item = new()
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                UnitPrice = ci.Product.CurrentPrice,
                Product = ci.Product,
                Order = order
            };
            await _orderRepository.AddOrderItemAsync(item);
        }

        return new OrderDto()
        {
            Order = order,
            TotalQuantity = quantity,
            TotalAmount = totalPrice,
            Status = order.Status,
            DeliveryAddresses = addresses.ToList(),
            PaymentMethods = paymentMethods.ToList()
        };
    }

    public async Task<OrderDto> UpdateOrderAsync(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new ApiException(ErrorCodes.UserNotFound, "User not found", StatusCodes.Status404NotFound);
        }

        var addresses = await _userRepository.GetUserDeliveryAddressesAsync(user);
        if (addresses == null)
        {
            throw new ApiException(ErrorCodes.UserNotFound, "User not found", StatusCodes.Status404NotFound);
        }
        
        var paymentMethods = await _userRepository.GetUserPaymentMethodsAsync(user);
        if (paymentMethods == null)
        {
            throw new ApiException(ErrorCodes.UserNotFound, "User not found", StatusCodes.Status404NotFound);
        }
        
        var cart = await ValidateCartAsync(userId);

        var order = await _orderRepository.GetPendingOrderByUserIdAsync(user.Id);
        if (order == null)
        {
            throw new ApiException(ErrorCodes.OrderNotFound, "Order not found", StatusCodes.Status404NotFound);
        }
        
        int quantity = 0;
        decimal totalPrice = 0;
        foreach (var ci in cart.Items)
        {
            var product = await _productRepository.GetByIdAsync(ci.ProductId);
            if (product == null)
            {
                throw new ApiException(ErrorCodes.ProductNotFound, "Product not found", StatusCodes.Status404NotFound);
            }
            
            quantity += ci.Quantity;
            totalPrice += product.CurrentPrice * ci.Quantity;
        }

        if (quantity == 0)
        {
            throw new ApiException(ErrorCodes.CartNotFound, "Cart is empty", StatusCodes.Status404NotFound);
        }

        await _orderRepository.RemoveOrderItemsAsync(order);
        order.TotalAmount = totalPrice;
        order.UpdatedAt = DateTime.Now;
        order.OrderItems = new List<OrderItem>();
        foreach (var ci in cart.Items)
        {
            OrderItem item = new()
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                UnitPrice = ci.Product.CurrentPrice,
                Product = ci.Product,
                Order = order
            };
            await _orderRepository.AddOrderItemAsync(item);
        }

        await _orderRepository.UpdateOrderAsync(order);
        
        return new OrderDto()
        {
            Order = order,
            TotalQuantity = quantity,
            TotalAmount = totalPrice,
            Status = order.Status,
            DeliveryAddresses = addresses.ToList(),
            PaymentMethods = paymentMethods.ToList()
        };
    }

    public async Task<CompletedOrderDto> CompleteOrderAsync(Guid userId, 
        Guid paymentMethodId, 
        Guid deliveryAddressId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new ApiException(ErrorCodes.UserNotFound, "User not found", StatusCodes.Status404NotFound);
        }
        
        var addresses = await _userRepository.GetUserDeliveryAddressesAsync(user);
        var address = await _userRepository.GetDeliveryAddressByIdAsync(deliveryAddressId);
        if (address == null || !addresses.Contains(address) || addresses == null)
        {
            throw new ApiException(ErrorCodes.UserNotFound, "Delivery address not found", StatusCodes.Status404NotFound);
        }
        
        var paymentMethods = await _userRepository.GetUserPaymentMethodsAsync(user);
        var paymentMethod = await _userRepository.GetPaymentMethodByIdAsync(paymentMethodId);
        if (paymentMethods == null || paymentMethod == null || !paymentMethods.Contains(paymentMethod))
        {
            throw new ApiException(ErrorCodes.UserNotFound, "Payment method not found", StatusCodes.Status404NotFound);
        }
        
        if (paymentMethod.ExpirationDate < DateOnly.FromDateTime(DateTime.Now))
        {
            throw new ApiException(ErrorCodes.ValidationError, "Payment method is expired",
                StatusCodes.Status406NotAcceptable);
        }
        
        var cart = await ValidateCartAsync(userId);
        var order = await _orderRepository.GetPendingOrderByUserIdAsync(user.Id);
        if (order == null)
        {
            throw new ApiException(ErrorCodes.OrderNotFound, "Order not found", StatusCodes.Status404NotFound);
        }

        foreach (var item in order.OrderItems)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
            {
                throw new ApiException(ErrorCodes.ProductNotFound, "Product not found", StatusCodes.Status404NotFound);
            }

            if (product.StockQuantity < item.Quantity)
            {
                throw new ApiException(ErrorCodes.ProductOutOfStock, "Product is out of stock", StatusCodes.Status409Conflict);
            }
            
            _productRepository.PurchaseProduct(product.Id, item.Quantity);
            _productRepository.Update(product);
        }

        await _cartRepository.ClearCartAsync(cart.Id);
        
        order.Status = OrderStatus.Confirmed;
        order.ConfirmedAt = DateTime.Now;
        order.DeliveryAddress = address;
        order.PaymentMethod = paymentMethod;
        await _orderRepository.UpdateOrderAsync(order);
        return new CompletedOrderDto()
        {
            Order = order,
            TotalQuantity = order.OrderItems.Sum(oi => oi.Quantity),
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            DeliveryAddress = address,
            PaymentMethod = paymentMethod
        };
    }

    public async Task<CompletedOrderDto> CancelOrderAsync(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new ApiException(ErrorCodes.UserNotFound, "User not found", StatusCodes.Status404NotFound);
        }
        
        var cart = await ValidateCartAsync(userId);
        var order = await _orderRepository.GetPendingOrderByUserIdAsync(user.Id);
        if (order == null)
        {
            throw new ApiException(ErrorCodes.OrderNotFound, "Order not found", StatusCodes.Status404NotFound);
        }

        await _cartRepository.ClearCartAsync(cart.Id);
        order.Status = OrderStatus.Cancelled;
        order.CanceledAt = DateTime.Now;
        await _orderRepository.UpdateOrderAsync(order);

        return new CompletedOrderDto()
        {
            Order = order,
            TotalQuantity = order.OrderItems.Sum(oi => oi.Quantity),
            TotalAmount = order.TotalAmount,
            Status = order.Status
        };
    }

    public async Task<List<CompletedOrderDto>> GetOrdersAsync(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new ApiException(ErrorCodes.UserNotFound, "User not found", StatusCodes.Status404NotFound);
        }
        
        var orders = await _orderRepository.GetOrdersAsync(user.Id);
        if (orders == null)
        {
            throw new ApiException(ErrorCodes.OrderNotFound, "Orders not found", StatusCodes.Status404NotFound);
        }
        
        List<CompletedOrderDto> completedOrders = new List<CompletedOrderDto>();
        foreach (var order in orders)
        {
            completedOrders.Add(new CompletedOrderDto()
            {
                Order = order,
                TotalQuantity = order.OrderItems.Sum(oi => oi.Quantity),
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                CompletedAt = order.ConfirmedAt,
                CanceledAt = order.CanceledAt,
                DeliveryAddress = order.DeliveryAddress,
                PaymentMethod = order.PaymentMethod
            });
        }
        return completedOrders;   
    }
    
    private async Task<Cart> ValidateCartAsync(Guid userId)
    {
        var cart = await _cartRepository.GetCartByUserIdAsync(userId);
        if (cart == null)
        {
            throw new ApiException(ErrorCodes.CartNotFound, "Cart not found", StatusCodes.Status404NotFound);
        }

        if (await _cartRepository.IsCartEmptyAsync(cart.Id))
        {
            throw new ApiException(ErrorCodes.ProductNotFound, "Cart is empty", StatusCodes.Status404NotFound);
        }
        
        return cart;
    }
    
}