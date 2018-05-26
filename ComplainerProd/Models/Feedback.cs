using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplainerProd.Models 
{
    public class Feedback :INotifyPropertyChanged
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

        private int upvote;

        public int Upvote
        {
            get
            {
                return upvote;
            }
            set
            {
                SetField(ref upvote, value, "Upvote");
            }
        }

        private int downvote;

        public int Downvote
        {
            get
            {
                return downvote;
            }
            set
            {
                SetField(ref downvote, value, "Downvote");
            }
        }
        private bool upvoteEnable = false;

        public bool UpvoteEnable
        {
            get
            {
                return upvoteEnable;
            }
            set
            {
                SetField(ref upvoteEnable, value, "UpvoteEnable");
            }
        }

        private bool downvoteEnable = false;

        public bool DownvoteEnable
        {
            get
            {
                return downvoteEnable;
            }
            set
            {
                SetField(ref downvoteEnable, value, "DownvoteEnable");
            }
        }

        public string Id { get; set; }

        private bool deleted = false;
        [JsonProperty(PropertyName ="deleted")]
        public bool Deleted
        {
            get
            {
                return deleted;
            }
            set
            {
                SetField(ref deleted, value, "Deleted");
            }
        }

        private DateTime createdAt;
        [JsonProperty(PropertyName = "CreatedDate")]
        public DateTime CreatedAt
        {
            get
            {
                return createdAt.ToLocalTime();
            }
            set
            {
                SetField(ref createdAt, value, "CreatedAt");
            }
        }

        private string channelName = "";
        [JsonProperty(PropertyName = "channelName")]
        public string ChannelName
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

        private int commentCount = 0;
        [JsonProperty(PropertyName = "CommentCount")]
        public int CommentCount
        {
            get
            {
                return commentCount;
            }
            set
            {
                SetField(ref commentCount, value, "CommentCount");
            }
        }

        [JsonProperty(PropertyName = "userID")]
        public string UserID { get; set; }

        [JsonProperty(PropertyName = "channelID")]
        public int ChannelID { get; set; }

        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }

        [JsonProperty(PropertyName = "action")]
        public int Action { get; set; }

        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }

        [JsonProperty(PropertyName = "attachedImage")]
        public string AttachedImage { get; set; }
    }
}
