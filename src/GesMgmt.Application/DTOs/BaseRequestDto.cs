namespace GesMgmt.Application.DTOs
{
    public abstract class BaseRequestDto
    {
        public string Language { get; set; }
        public required string MerchantCode { get; set; }
        public required string User { get; set; }
        public required string Origin { get; set; }
    }
}
