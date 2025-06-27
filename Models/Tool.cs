namespace SixxonAPI.Models
{
    public class Tool
    {
        public string Id { get; set; }
        public string Tool_Code { get; set; }
        public string Tool_Name { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public int Borrow_Days { get; set; }
        public string Remark { get; set; }
        public string Barcode { get; set; }
        public string Receiver { get; set; }
        public DateTime? ExpectedReturnDate { get; set; } 
    }

}
