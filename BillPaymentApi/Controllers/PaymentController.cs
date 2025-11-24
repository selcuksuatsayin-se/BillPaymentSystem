using BillPaymentApi.Data;
using BillPaymentApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace BillPaymentApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [SwaggerTag("Web Site")]
    public class PaymentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PaymentController(AppDbContext context)
        {
            _context = context;
        }

        // REQUIREMENT: Web Site - Pay Bill
        [HttpPost("pay")]
        public async Task<IActionResult> PayBill([FromBody] PaymentDto payment)
        {
            // 1. Find the invoice (based on Subscriber Number and Month information)
            var bill = await _context.Bills
                .Include(b => b.Subscriber)
                .FirstOrDefaultAsync(b => b.Subscriber.PhoneNumber == payment.PhoneNumber && b.Month == payment.Month && !b.IsPaid);

            if (bill == null)
            {
                return NotFound("No invoice to pay found or already paid.");
            }

            // If the subscriber tries to pay more than he owes, block him/her
            if (payment.PaymentAmount > bill.Amount)
            {
                return BadRequest(new
                {
                    Error = "Ödeme tutarı mevcut borçtan fazla olamaz.",
                    CurrentDebt = bill.Amount, // Show the current debt
                    AttemptedPayment = payment.PaymentAmount
                });
            }

            string statusMessage;

            if (payment.PaymentAmount == bill.Amount)
            {
                // Full payment
                bill.IsPaid = true;
                bill.Amount = 0; // No more debt
                statusMessage = "Successful - The debt was paid in full.";
            }
            else
            {
                // Partial payment
                bill.Amount -= payment.PaymentAmount; // Update remaining amount
                bill.IsPaid = false; // still in debt
                statusMessage = $"Successful - Partial payment received. Remaining debt: {bill.Amount}";
            }

            // Simply return a response for Transaction log

            await _context.SaveChangesAsync();

            return Ok(new
            {
                PaymentStatus = "Successful",
                TransactionStatus = statusMessage,
                RemainingAmount = bill.Amount
            });
        }
    }
}