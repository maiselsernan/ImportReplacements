using System;

namespace ImportReplacement.Models
{
    public class ResponseToFile
    {
        public string FullName { get; set; }
        public long MeterId { get; set; }
        public string Address { get; set; }
        public DateTime DateTime { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public long OldNumber { get; set; }
        public long OldReading { get; set; }
        public string OldImage { get; set; }
        public long NewNumber { get; set; }
        public long Reading { get; set; }
        public string NewImage { get; set; }
        public int Diameter { get; set; }
        public string Comment { get; set; }
    }
}
