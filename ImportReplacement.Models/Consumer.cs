namespace ImportReplacement.Models
{
    public class Consumer
    {
        public int ID { get; set; }
        public int SiteID { get; set; }
        public long? Channel { get; set; }
        public long? BilingId { get; set; }
        public long? MeterNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public RequestTypes replacement_type { get; set; }

    }
}
