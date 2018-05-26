using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplainerProd.Models
{
    public class Comment
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


        private bool officialReplied;

        public bool OfficialReplied
        {
            get
            {
                return officialReplied;
            }
            set
            {
                SetField(ref officialReplied, value, "OfficialReplied");
            }
        }

        public string id { get; set; }

        public bool Deleted { get; set; }

        [JsonProperty (PropertyName="userID")]
        public string UserID { get; set; }

        [JsonProperty(PropertyName = "feedbackID")]
        public string FeedbackID { get; set; }

        [JsonProperty(PropertyName = "Content")]
        public string CommentContent { get; set; }

        [JsonProperty(PropertyName = "CreatedDate")]
        public DateTime CreatedAt { get; set; }


    }
}
