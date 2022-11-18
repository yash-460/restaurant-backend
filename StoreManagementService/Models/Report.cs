namespace StoreManagementService.Models
{
    public class Report
    {
        public string? ProductName { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
