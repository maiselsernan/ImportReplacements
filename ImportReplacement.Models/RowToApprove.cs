using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ImportReplacement.Models
{
    public class RowToApprove : IDetailsProvider, INotifyPropertyChanged
    {
        private bool _approved;
        private string _meterType;
        private string _meterManufacturer;
        private string _meterModel;
        private long? _channelId;
        private bool? _billable;
        private int _reasonId;
        private string _chargeDescription;
        private long _oldNumber;
        private long _newNumber;
        private int _reading;
        private int _oldReading;
    
        public float? longitude { get; set; }
        public float? latitude { get; set; }
        public long CommandId { get; set; }
      
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? SiteId { get; set; }
        public string SiteName { get; set; }
        public string OldImage { get; set; }
        public string NewImage { get; set; }

        public long? Consumer { get; set; }
        public long? NonAmrConsumer { get; set; }
        public string? Diameter { get; set; }
        public long Code { get; set; }
        public string Comment { get; set; }
        public long? ChannelMeterNumber { get; set; }
        public DateTime? Expired { get; set; }
        public int User { get; set; }

        public int Reading {
            get => _reading;
            set
            {
                if (value == _reading) return;
                _reading = value;
                OnPropertyChanged();
            }
        }
        public int OldReading
        {
            get => _oldReading;
            set
            {
                if (value == _oldReading) return;
                _oldReading = value;
                OnPropertyChanged();
            }
        }
        public long OldNumber
        {
            get => _oldNumber;
            set
            {
                if (value == _oldNumber) return;
                _oldNumber = value;
                OnPropertyChanged();
            }
        }



        public long NewNumber 
        {
            get => _newNumber;
            set
            {
                if (value == _newNumber) return;
                _newNumber = value;
                OnPropertyChanged();
            }
        }





        public long? ChannelId
        {
            get => _channelId;
            set
            {
                if (value == _channelId) return;
                _channelId = value;
                OnPropertyChanged();
            }
        }

        public string MeterModel
        {
            get => _meterModel;
            set
            {
                if (value == _meterModel) return;
                _meterModel = value;
                OnPropertyChanged();
            }
        }
        public string MeterManufacturer
        {
            get => _meterManufacturer;
            set
            {
                if (value == _meterManufacturer) return;
                _meterManufacturer = value;
                OnPropertyChanged();
            }
        }

        public string MeterType
        {
            get => _meterType;
            set
            {
                if (value == _meterType) return;
                _meterType = value;
                OnPropertyChanged();
            }
        }


        public bool Approved
        {
            get => _approved;
            set
            {
                if (value == _approved) return;
                _approved = value;
                OnPropertyChanged();
            }
        }

        public DateTime ReplaceDate { get; set; }
        public bool DisplayBillable
        {
            get => Billable ?? Expired < ReplaceDate;
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

       

        public bool IsDisabled => !Approved && (!ChannelId.HasValue || ChannelId.Value == 0 || NewNumber != ChannelMeterNumber);
        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
