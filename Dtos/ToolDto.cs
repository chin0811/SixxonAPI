namespace SixxonAPI.Dtos
{
    public class ToolDto
    {
        public string ToolCode { get; set; }
        public string ToolName { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public int BorrowDays { get; set; }
        public string Remark { get; set; }
        public string Barcode { get; set; }
        public string Receiver { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }  
    }
    public class CreateToolDto
    {
        public string ToolCode { get; set; }
        public string ToolName { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public int BorrowDays { get; set; }
        public string Remark { get; set; }
        public string Barcode { get; set; }
    }

}
