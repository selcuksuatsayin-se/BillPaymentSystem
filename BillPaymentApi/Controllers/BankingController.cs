using BillPaymentApi.Data;
using BillPaymentApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace BillPaymentApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    [SwaggerTag("Banking App")]
    public class BankingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BankingController(AppDbContext context)
        {
            _context = context;
        }

        // REQUIREMENT: Banking App - Query Bill
        [HttpGet("bills/{phoneNumber}")]
        public async Task<IActionResult> GetUnpaidBillsByPhone(string phoneNumber)
        {
            // 1. Find the subscriber by phone number
            var subscriber = await _context.Subscribers
                .FirstOrDefaultAsync(s => s.PhoneNumber == phoneNumber);

            if (subscriber == null)
            {
                return NotFound("No subscriber found for this phone number.");
            }

            // 2. Only bring UNPAID (!IsPaid) invoices
            var unpaidBills = await _context.Bills
                .Where(b => b.SubscriberId == subscriber.Id && !b.IsPaid)
                .Select(b => new BillResponseDto
                {
                    Id = b.Id,
                    Month = b.Month,
                    Amount = b.Amount,
                    IsPaid = b.IsPaid
                })
                .ToListAsync();

            // If there is no debt, return an empty list and give a message
            if (!unpaidBills.Any())
            {
                return Ok(new { Message = "Bu abonenin ödenmemiş faturası bulunmamaktadır." });
            }

            return Ok(unpaidBills);
        }
    }
}