namespace BillPaymentApi.DTOs
{
    public class BillResponseDto
    {
        public int Id { get; set; }
        public string Month { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
    }
}