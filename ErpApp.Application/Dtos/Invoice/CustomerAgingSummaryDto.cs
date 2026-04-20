namespace ErpApp.Application.Dtos.Invoice
{
    public class CustomerAgingSummaryDto
    {
        public int CustomerId { get; set; }
        public decimal Current { get; set; }
        public decimal Bucket1To30 { get; set; }
        public decimal Bucket31To60 { get; set; }
        public decimal Bucket61To90 { get; set; }
        public decimal BucketOver90 { get; set; }
        public decimal TotalOutstanding { get; set; }
    }
}
