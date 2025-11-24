namespace BillPaymentApi.DTOs
{
    public class PaymentDto
    {
        public string PhoneNumber { get; set; }
        public string Month { get; set; }
        public decimal PaymentAmount { get; set; }
    }
}