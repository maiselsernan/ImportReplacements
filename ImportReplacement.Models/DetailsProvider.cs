using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using ImportReplacement.Models.Annotations;

namespace ImportReplacement.Models
{
    public interface IDetailsProvider
    {
        public bool DisplayBillable {get; set;}
        public int ReasonId {get; set;}
        public string ChargeDescription {get; set;}
        public long CommandId { get; set; }
        public bool? Billable { get; set; }
        public string OldImage { get; set; }
        public string NewImage { get; set; }
        public float? longitude { get; set; }
        public float? latitude { get; set; }
        public int? SiteId { get; set; }

    }
    public class DetailsProvider :IDetailsProvider, INotifyPropertyChanged
    {
        private bool? _billable;
        private int _reasonId;
        private string _chargeDescription;
        public long CommandId { get; set; }
        public string OldImage { get; set; }
        public string NewImage { get; set; }
        public float? longitude { get; set; }
        public float? latitude { get; set; }
        public int? SiteId { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public string ChargeDescription
        {
            get => _chargeDescription;
            set
            {
                if (value == _chargeDescription) return;
                _chargeDescription = value;
                OnPropertyChanged();
            }
        }

        public bool DisplayBillable
        {
            get => Billable != null && Billable.Value;
            set => Billable = value;
        }

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
        public int ReasonId
        {
            get => _reasonId;
            set
            {
                if (value == _reasonId) return;
                _reasonId = value;
                OnPropertyChanged();
            }
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
