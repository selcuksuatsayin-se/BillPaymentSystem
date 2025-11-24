using BillPaymentApi.Data;
using BillPaymentApi.DTOs;
using BillPaymentApi.Entities;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Globalization;

namespace BillPaymentApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [SwaggerTag("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // REQUIREMENT: Admin Add Bill - Batch (.csv file)
        [HttpPost("bills/batch")]
        public async Task<IActionResult> UploadBillCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Upload a valid CSV file.");

            using (var stream = new StreamReader(file.OpenReadStream()))
            using (var csv = new CsvReader(stream, CultureInfo.InvariantCulture))
            {
                // Read CSV
                var records = csv.GetRecords<BillCsvDto>().ToList();

                foreach (var record in records)
                {
                    // 1. Is there a subscriber with this phone number?
                    var subscriber = _context.Subscribers
                        .FirstOrDefault(s => s.PhoneNumber == record.SubscriberPhoneNumber);

                    // If there is no subscriber, who will we bill?
                    // If there is no subscriber as per the scenario, we can automatically create one
                    if (subscriber == null)
                    {
                        subscriber = new Subscriber
                        {
                            PhoneNumber = record.SubscriberPhoneNumber,
                            Role = "User",
                        };
                        _context.Subscribers.Add(subscriber);
                        await _context.SaveChangesAsync(); // Save for creating ID
                    }

                    // 2 Create the invoice and link it to the subscriber
                    var newBill = new Bill
                    {
                        SubscriberId = subscriber.Id,
                        Amount = record.Amount,
                        Month = record.Month,
                        IsPaid = false
                    };

                    _context.Bills.Add(newBill);
                }

                await _context.SaveChangesAsync();
                return Ok(new { Message = $"{records.Count} invoices uploaded successfully." });
            }
        }


        // REQUIREMENT: Admin - Add Bill (Single)
        [HttpPost("bills")]
        public async Task<IActionResult> AddSingleBill([FromBody] BillAddDto billDto)
        {
            // 1. Find subscriber
            var subscriber = _context.Subscribers.FirstOrDefault(s => s.PhoneNumber == billDto.SubscriberPhoneNumber);

            if (subscriber == null)
            {
                // If there is no subscriber as per the scenario, we can automatically create one
                subscriber = new Subscriber
                {
                    PhoneNumber = billDto.SubscriberPhoneNumber,
                    Role = "User",
                };
                _context.Subscribers.Add(subscriber);
                await _context.SaveChangesAsync();
            }

            // 2. Create bill
            var newBill = new Bill
            {
                SubscriberId = subscriber.Id,
                Amount = billDto.Amount,
                Month = billDto.Month,
                IsPaid = false
            };

            _context.Bills.Add(newBill);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Fatura başarıyla eklendi.", BillId = newBill.Id });
        }
    }
}