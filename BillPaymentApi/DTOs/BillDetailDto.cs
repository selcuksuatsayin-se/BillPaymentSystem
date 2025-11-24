namespace BillPaymentApi.DTOs
{
    public class BillDetailDto
    {
        // Pagination information
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

        // Returned data
        public List<BillResponseDto> Bills { get; set; }
    }
}