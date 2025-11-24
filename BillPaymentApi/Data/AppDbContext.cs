using Microsoft.EntityFrameworkCore;
using BillPaymentApi.Entities;

namespace BillPaymentApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<ApiLog> ApiLogs { get; set; }
    }
}