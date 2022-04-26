namespace BoxTrackLabel.API.Models
{
    public class LabelToPrint
    {
        public string Description { get; set; }
        public string Qr { get; set; } 
        public string Ean { get; set; } 
        public string Address { get; set; } 
        public string Quantity { get; set; } 
        public string Message { get; set; }
        public string Country { get; set; }
        public string CountryEn { get; set; }
        public string Module { get; set; }
        public string DataMatrixCode { get; set; }
    }
}