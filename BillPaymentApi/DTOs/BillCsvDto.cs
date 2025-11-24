namespace BillPaymentApi.DTOs
{
    public class BillCsvDto
    {
        // CSV headers must match these
        public string SubscriberPhoneNumber { get; set; }
        public decimal Amount { get; set; }
        public string Month { get; set; }
    }
}