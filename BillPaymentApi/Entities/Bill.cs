namespace BillPaymentApi.Entities
{
    public class Bill
    {
        public int Id { get; set; }
        public int SubscriberId { get; set; }
        public string Month { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; } = false;
        public Subscriber Subscriber { get; set; }
    }
}