using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ImportReplacement.Models
{
   public class RowToCharge  : IDetailsProvider, INotifyPropertyChanged
    {
        public int? SiteId { get; set; }
        public string SiteName { get; set; }
        public long BilingID { get; set; }
        public string Address { get; set; }
        public DateTime DateTime { get; set; }
        public float? longitude { get; set; }
        public float? latitude { get; set; }
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
        public bool DisplayBillable
        {
            get => Billable ?? Expired < ReplaceDate;
            set => Billable = value;
        }
        private bool? _billable;

        public bool? Billable
        {
            get => _billable;
            set
            {
                if (value == _billable) return;
                _billable = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayBillable));
            }
        }
        private bool _charged;

        public bool Charged
        {
            get => _charged;
            set
            {
                if (value == _charged) return;
                _charged = value;
                OnPropertyChanged();
            }
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
