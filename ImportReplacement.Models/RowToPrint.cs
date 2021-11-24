using System;
using System.Collections.Generic;

namespace ImportReplacement.Models
{
    public class RowToPrint
    {
        public int SiteID { get; set; }
        public string SiteName { get; set; }
        public long BillingID { get; set; }
        public string Address { get; set; }
        public DateTime DateTime { get; set; }
        public float? Longitude { get; set; }
        public float? Latitude { get; set; }
        public long OldNumber { get; set; }
        public long NewNumber { get; set; }
        public long OldReading { get; set; }
        public string OldImage { get; set; }
        public string NewImage { get; set; }
        public long Reading { get; set; }
        public string Comment { get; set; }
        public long Code { get; set; }
        public long Diameter { get; set; }
        public long ConsumerId { get; set; }
        public string CodeDescription { get; set; }
        public string ChargeDescription { get; set; }
        public int ReasonId { get; set; }
        public string ReplacementReason { get; set; }
        public long CommandId { get; set; }
        public DateTime ReplaceDate { get; set; }
        public DateTime? Expired { get; set; }
        public bool Charged { get; set; } 
        public  List<ConsumerElement> Elements { get; set; }

        
    }
    
}
