namespace WSMS.Models
{
    public class Customer
    {
        public string? ID { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber1 { get; set; }
        public string? PhoneNumber2 { get; set; }
        public string? PhoneNumber3 { get; set; }
        public string? MainDiraction { get; set; }
        public string? SubDiraction { get; set; } 
        public string? Address { get; set; }
        public bool IsSelected { get; set; } = false;
    }
}
