namespace BillPaymentApi.DTOs
{
    public class BillAddDto
    {
        public string SubscriberPhoneNumber { get; set; }
        public decimal Amount { get; set; }
        public string Month { get; set; }
    }
}