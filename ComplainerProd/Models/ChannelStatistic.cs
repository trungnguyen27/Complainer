using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplainerProd.Models
{
    public class ChannelStatistic :INotifyPropertyChanged
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


        private int totalFeedbacks;

        public int TotalFeedbacks
        {
            get
            {
                return totalFeedbacks;
            }
            set
            {
                SetField(ref totalFeedbacks, value, "TotalFeedbacks");
            }
        }

        private int todayFeedbacks;

        public int TodayFeedbacks
        {
            get
            {
                return todayFeedbacks;
            }
            set
            {
                SetField(ref todayFeedbacks, value, "TodayFeedbacks");
            }
        }

        private int todayVotes;

        public int TodayVotes
        {
            get
            {
                return todayVotes;
            }
            set
            {
                SetField(ref todayVotes, value, "TodayVotes");
            }
        }

        private int todayUpvotes;

        public int TodayUpvotes
        {
            get
            {
                return todayUpvotes;
            }
            set
            {
                SetField(ref todayUpvotes, value, "TodayUpvotes");
            }
        }

        private int todayDownvotes;

        public int TodayDownvotes
        {
            get
            {
                return todayDownvotes;
            }
            set
            {
                SetField(ref todayDownvotes, value, "TodayDownvotes");
            }
        }
        public string id { get; set; }

        public string ChannelID { get; set; }
    }
}
