using AutoMapper;
using ECommerceAPI.DTO;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Data.Repositories
{
    public interface IPaymentRepository
    {
        Task<PaymentResponseDTO> MakePaymentAsync(PaymentDTO paymentDto);
        Task<UpdatePaymentResponseDTO> UpdatePaymentStatusAsync(int paymentId, string newStatus);
        Task<Payment?> GetPaymentDetailsAsync(int paymentId);
    }

    public class PaymentRepository : IPaymentRepository
    {
        private readonly EcommerceContext _context;
        private readonly IMapper _mapper;

        public PaymentRepository(EcommerceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PaymentResponseDTO> MakePaymentAsync(PaymentDTO paymentDto)
        {
            var paymentResponseDTO = new PaymentResponseDTO();

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == paymentDto.OrderId && o.Status == "Pending");

            if (order == null)
            {
                // If order is not pending, return an error message
                paymentResponseDTO.Message = "Order either does not exist or is not in a pending state.";
                return paymentResponseDTO;
            }

            // Validate that the payment amount matches the order amount
            if (order.TotalAmount != paymentDto.Amount)
            {
                paymentResponseDTO.Message = "Payment amount does not match the order total.";
                return paymentResponseDTO;
            }

            // Create a new payment entity
            var payment = new Payment
            {
                OrderId = paymentDto.OrderId,
                Amount = paymentDto.Amount,
                Status = "Pending", // Initial payment status is 'Pending'
                PaymentType = paymentDto.PaymentType,
                PaymentDate = DateTime.Now
            };

            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();

            // Simulate interaction with a payment gateway
            payment.Status = SimulatePaymentGatewayInteraction(paymentDto);

            // Update the payment status based on the payment gateway's response
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();

            // Map the payment details to PaymentResponseDTO using AutoMapper
            paymentResponseDTO = _mapper.Map<PaymentResponseDTO>(payment);
            paymentResponseDTO.IsCreated = true;
            paymentResponseDTO.Message = $"Payment Processed with Status {payment.Status}";

            return paymentResponseDTO;

        }

        public async Task<UpdatePaymentResponseDTO> UpdatePaymentStatusAsync(int paymentId, string newStatus)
        {
            var updatePaymentResponseDTO = new UpdatePaymentResponseDTO
            {
                PaymentId = paymentId
            };

            var payment = await _context.Payments.Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            if (payment == null)
            {
                throw new Exception("Payment record not found.");
            }

            if (!IsValidStatusTransition(payment.Status, newStatus, payment.Order.Status))
            {
                updatePaymentResponseDTO.IsUpdated = false;
                updatePaymentResponseDTO.Message = $"Invalid status transition from {payment.Status} to {newStatus} for order status {payment.Order.Status}.";
                return updatePaymentResponseDTO;
            }

            // Update payment status
            payment.Status = newStatus;
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();

            // Populate the update response DTO with updated data
            updatePaymentResponseDTO.CurrentStatus = payment.Status;
            updatePaymentResponseDTO.UpdatedStatus = newStatus;
            updatePaymentResponseDTO.IsUpdated = true;
            updatePaymentResponseDTO.Message = $"Payment Status Updated to {newStatus}";

            return updatePaymentResponseDTO;
        }

        public async Task<Payment?> GetPaymentDetailsAsync(int paymentId)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == paymentId);

        }

        private string SimulatePaymentGatewayInteraction(PaymentDTO paymentDto)
        {
            // Simulate different responses based on the payment type or other logic
            switch (paymentDto.PaymentType)
            {
                case "COD":
                    return "Completed";  // COD is usually confirmed immediately if used
                case "CC":
                    return "Completed";  // Assuming credit card payments are processed immediately
                case "DC":
                    return "Failed";     // Simulating a failure for demonstration purposes
                default:
                    return "Completed";  // Default to completed for simplicity in this example
            }
        }
        private bool IsValidStatusTransition(string currentStatus, string newStatus, string orderStatus)
        {
            // Completed payments cannot be modified unless it's a refund for a returned order.
            if (currentStatus == "Completed" && newStatus != "Refund")
            {
                return false;
            }

            // Only pending payments can be cancelled.
            if (currentStatus == "Pending" && newStatus == "Cancelled")
            {
                return true;
            }

            // Refunds should only be processed for returned orders.
            if (currentStatus == "Completed" && newStatus == "Refund" && orderStatus != "Returned")
            {
                return false;
            }

            // Payments should only be marked as failed if they are not completed or cancelled.
            if (newStatus == "Failed" && (currentStatus == "Completed" || currentStatus == "Cancelled"))
            {
                return false;
            }

            // Assuming 'Pending' payments become 'Completed' when the order is shipped or services are rendered.
            if (currentStatus == "Pending" && newStatus == "Completed" && (orderStatus == "Shipped" || orderStatus == "Confirmed"))
            {
                return true;
            }

            // Handle other generic cases or add more specific business rule checks
            return true;
        }
    }


}
