namespace BillPaymentApi.Entities
{
    public class ApiLog
    {
        public int Id { get; set; }
        public string HttpMethod { get; set; }
        public string Path { get; set; }
        public DateTime RequestTime { get; set; }
        public long ResponseLatencyMs { get; set; }
        public int StatusCode { get; set; }
        public string? IpAddress { get; set; }
        public string? RequestHeaders { get; set; }
        public long? RequestSize { get; set; }
        public long? ResponseSize { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}