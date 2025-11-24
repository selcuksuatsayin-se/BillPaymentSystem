using BillPaymentApi.Data;
using BillPaymentApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Swashbuckle.AspNetCore.Annotations;

namespace BillPaymentApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    [SwaggerTag("Mobile Provider App")]
    public class SubscriberController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SubscriberController(AppDbContext context)
        {
            _context = context;
        }

        // 1. Query Bill
        // GET: api/v1/Subscriber/bills?month=2024-10
        [HttpGet("bills")]
        public IActionResult GetBills([FromQuery] string? month)
        {
            // Get the ID of the subscriber who logged in from the Token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            int subscriberId = int.Parse(userIdClaim.Value);

            // Prepare the query
            var query = _context.Bills.Where(b => b.SubscriberId == subscriberId && !b.IsPaid);

            // Filter it by the month parameter
            if (!string.IsNullOrEmpty(month))
            {
                query = query.Where(b => b.Month == month);
            }

            var bills = query.Select(b => new BillResponseDto
            {
                Id = b.Id,
                Amount = b.Amount,
                IsPaid = b.IsPaid,
                Month = b.Month
            }).ToList();

            return Ok(new { BillTotal = bills.Sum(x => x.Amount), Bills = bills });
        }

        // 2. Query Bill Detailed
        // GET: api/v1/Subscriber/bills/detailed?page=1&pageSize=10
        [HttpGet("bills/detailed")]
        public async Task<IActionResult> GetBillsDetailed([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int subscriberId = int.Parse(userIdClaim.Value);

            var query = _context.Bills.Where(b => b.SubscriberId == subscriberId);

            // Total number of records (For paging calculation)
            int totalCount = await query.CountAsync();

            // Paging logic: (Page - 1) * Skip as much as the quantity, take as much as the quantity.
            var bills = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BillResponseDto
                {
                    Id = b.Id,
                    Amount = b.Amount,
                    IsPaid = b.IsPaid,
                    Month = b.Month
                })
                .ToListAsync();

            var response = new BillDetailDto
            {
                CurrentPage = page,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Bills = bills
            };

            return Ok(response);
        }
    }
}