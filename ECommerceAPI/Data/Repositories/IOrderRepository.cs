using AutoMapper;
using ECommerceAPI.DTO;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Data.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllOrdersAsync(string Status);
        Task<CreateOrderResponseDTO> CreateOrderAsync(OrderDTO orderDto);
        Task<ConfirmOrderResponseDTO> ConfirmOrderAsync(int orderId);
        Task<OrderStatusResponseDTO> UpdateOrderStatusAsync(int orderId, string newStatus);
        Task<Order?> GetOrderDetailsAsync(int orderId);
    }

    public class OrderRepository : IOrderRepository
    {
        private readonly EcommerceContext _context;
        private readonly IMapper _mapper;

        public OrderRepository(EcommerceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Method to fetch orders based on Status
        public Task<List<Order>> GetAllOrdersAsync(string Status)
        {
            var orders = _context.Orders.Include(p => p.Payments)
                .Include(o => o.OrderItems)
                .Where(O => O.Status == Status)
                .ToListAsync();
            return orders;
        }

        //Create the Order with Pending State
        public async Task<CreateOrderResponseDTO> CreateOrderAsync(OrderDTO orderDto)
        {
            decimal totalAmount = 0m;  // Variable to store total order amount
            List<OrderItem> validatedItems = new List<OrderItem>(); // List to hold validated order items

            // Loop through each item in the order DTO to validate it
            foreach (var itemDto in orderDto.Items)
            {
                // Fetch the product from the database that matches the product ID and ensure it's not deleted
                var product = await _context.Products
                                            .Where(p => p.ProductId == itemDto.ProductId && !p.IsDeleted)
                                            .FirstOrDefaultAsync();

                // If product doesn't exist or there's insufficient quantity, return failure response
                if (product == null || product.Quantity < itemDto.Quantity)
                {
                    return new CreateOrderResponseDTO
                    {
                        IsCreated = false,
                        Message = $"Insufficient stock for product ID {itemDto.ProductId}"
                    };
                }

                // Calculate the total amount for this item and add it to the totalAmount
                totalAmount += product.Price * itemDto.Quantity;

                // Add the validated item to the order items list
                validatedItems.Add(new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    PriceAtOrder = product.Price // Store the price at the time of the order
                });
            }

            // Create a new order entity
            var order = new Order
            {
                CustomerId = orderDto.CustomerId, // Set customer ID
                TotalAmount = totalAmount,        // Set total amount for the order
                Status = "Pending",               // Set initial status of the order
                OrderDate = DateTime.Now,         // Set the current date as the order date
                OrderItems = validatedItems       // Assign the validated order items
            };

            // Add the new order to the database context
            _context.Orders.Add(order);

            // Commit the changes to the database (save the order)
            await _context.SaveChangesAsync();

            // Return success response with the newly created order's ID and status
            return new CreateOrderResponseDTO
            {
                OrderId = order.OrderId,
                Status = "Pending",
                IsCreated = true,
                Message = "Order created successfully"
            };
        }

        public async Task<ConfirmOrderResponseDTO> ConfirmOrderAsync(int orderId)
        {
            var order = await _context.Orders.Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return new ConfirmOrderResponseDTO
                {
                    IsConfirmed = false,
                    Message = "Order not found"
                };
            }

            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId && p.Status == "Completed");
            if (payment == null || payment.Amount != order.TotalAmount)
            {
                return new ConfirmOrderResponseDTO
                {
                    IsConfirmed = false,
                    Message = "Payment not completed or mismatch with order total"
                };
            }

            foreach (var item in order.OrderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                product.Quantity -= item.Quantity;
            }

            order.Status = "Confirmed";

            await _context.SaveChangesAsync();

            return new ConfirmOrderResponseDTO
            {
                IsConfirmed = true,
                Message = "Order confirmed successfully",
                OrderId = orderId
            };

        }

        public async Task<OrderStatusResponseDTO> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return new OrderStatusResponseDTO
                {
                    IsUpdated = false,
                    Message = "Order not found"
                };
            }

            if (!IsValidStatusTransition(order.Status, newStatus))
            {
                return new OrderStatusResponseDTO
                {
                    IsUpdated = false,
                    Message = $"Invalid status transition from {order.Status} to {newStatus}"
                };
            }

            order.Status = newStatus;
            await _context.SaveChangesAsync();

            return new OrderStatusResponseDTO
            {
                IsUpdated = true,
                Message = $"Order status updated to {newStatus}",
                OrderId = orderId
            };

        }

        public async Task<Order?> GetOrderDetailsAsync(int orderId)
        {
            var order = await _context.Orders.Include(o => o.OrderItems)
                .Include(p => p.Payments)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
            {
                return null;
            }

            return order;
        }


        private bool IsValidStatusTransition(string currentStatus, string newStatus)
        {
            switch (currentStatus)
            {
                case "Pending": return newStatus == "Processing" || newStatus == "Cancelled";
                case "Confirmed": return newStatus == "Processing";
                case "Processing": return newStatus == "Delivered";
                default: return false;
            }
        }

    }

}