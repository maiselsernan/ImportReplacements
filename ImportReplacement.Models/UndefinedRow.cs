using System;

namespace ImportReplacement.Models
{
   public class UndefinedRow 
    {
        public long command_id { get; set; }
        public string file { get; set; }
        public long? meter_id { get; set; }
        public string address { get; set; }
        public int? user { get; set; }
        public DateTime? datetime { get; set; }
        public float? longitude { get; set; }
        public float? latitude { get; set; }
        public long? old_number { get; set; }
        public string old_image { get; set; }

        public long? new_number { get; set; }
        public string new_image { get; set; }
        public string comment { get; set; }
        public long? code { get; set; }
        public string code_description { get; set; }
        public string? diameter { get; set; }
        public int ReasonId { get; set; }
        public string ChargeDescription { get; set; }
        public int? site_id { get; set; }
        public string ExistingConsumers { get; set; }
        private bool? _billable;

        public bool? Billable
        {
            get => _billable;
            set
            {
                if (value == _billable) return;
                _billable = value;
              
            }
        }
    }
}
