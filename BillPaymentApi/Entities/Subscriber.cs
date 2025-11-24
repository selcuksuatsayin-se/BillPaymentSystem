namespace BillPaymentApi.Entities
{
    public class Subscriber
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; } = "User";
        public List<Bill> Bills { get; set; }
    }
}