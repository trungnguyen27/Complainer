using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplainerProd.Models
{
    public class Channel :INotifyPropertyChanged
    {
        //boiler-plate
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }


        private string channelName;

        public string Name
        {
            get
            {
                return channelName;
            }
            set
            {
                SetField(ref channelName, value, "ChannelName");
            }
        }

        private string storageURL;

        public string StorageURL
        {
            get
            {
                return storageURL;
            }
            set
            {
                SetField(ref storageURL, value, "StorageURL");
            }
        }

        private string location;

        public string Location
        {
            get
            {
                return location;
            }
            set
            {
                SetField(ref location, value, "Location");
            }
        }


        private string accessCode;

        public string AccessCode
        {
            get
            {
                return accessCode;
            }
            set
            {
                SetField(ref accessCode, value, "AccessCode");
            }
        }

        private string phoneNumber;

        public string Phone
        {
            get
            {
                return phoneNumber;
            }
            set
            {
                SetField(ref phoneNumber, value, "PhoneNumber");
            }
        }

        private string about;

        public string About
        {
            get
            {
                return about;
            }
            set
            {
                SetField(ref about, value, "About");
            }
        }


        public int Id { get; set; }

        [JsonProperty(PropertyName = "userID")]
        public string UserId { get; set; }
    }
}
