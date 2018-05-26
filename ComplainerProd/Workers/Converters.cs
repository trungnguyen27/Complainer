using ComplainerProd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ComplainerProd.DataObject
{
    public class ActionConverter : IValueConverter
    {
        // This converts the DateTime object to the string to display.
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            // Retrieve the format string and use it to format the value.
            int actionIndex = int.Parse(value.ToString());

            if (App.ActionDictionary.ContainsKey(actionIndex))
                return App.ActionDictionary[actionIndex];
            // If the format string is null or empty, simply call ToString()
            // on the value.
            return "";
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class DateConverter : IValueConverter
    {
        // This converts the DateTime object to the string to display.
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            // Retrieve the format string and use it to format the value.
            var dateTime = (DateTime)value;
            if (dateTime == null)
                return "";

            var timespan = DateTime.Now - dateTime;
            var days = Math.Round(timespan.TotalDays);
            var hours = timespan.Hours;
            var minutes = timespan.Minutes;
            if (days > 2)
                return dateTime.Date.Day + "-" + dateTime.Date.Month + "-" + dateTime.Date.Year;
            else if (days > 1 && days <= 2)
                return "Cách đây " + days.ToString() + " ngày";
            else if (hours > 1) return "Cách đây " + hours.ToString() + " giờ";
            else if (minutes > 30) return "Cách đây " + minutes.ToString() + " phút";
            else return "Vài phút gần đây";
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class DeletedToVisibility : IValueConverter
    {
        // This converts the DateTime object to the string to display.
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            // Retrieve the format string and use it to format the value.
            bool b = (bool)value;
            if (b)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class DeletedToVisibilityReversed : IValueConverter
    {
        // This converts the DateTime object to the string to display.
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            // Retrieve the format string and use it to format the value.
            bool b = (bool)value;
            if (b)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class FromUserIDtoVisibility : IValueConverter
    {
        // This converts the DateTime object to the string to display.
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            // Retrieve the format string and use it to format the value.
            if (value is Feedback)
            {
                var fb = value as Feedback;
            }
            var id = value as string;

            if (id != null)
            {
                if (id == MainPage.instance.user.UserId)
                    return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class ReverseBooleanConverter : IValueConverter
    {
        // This converts the DateTime object to the string to display.
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            // Retrieve the format string and use it to format the value.

            var id = (bool)value;

            return !id;
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class FromActionToIndex : IValueConverter
    {
        // This converts the DateTime object to the string to display.
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            // Retrieve the format string and use it to format the value.
            var index = (int)value;

            if (App.ActionDictionary.ContainsKey(index))
            {
                return index;
            }
            else return 0;
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class FromCountToVisibility : IValueConverter
    {
        // This converts the DateTime object to the string to display.
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            // Retrieve the format string and use it to format the value.
            var index = (int)value;

            if (index <= 0)
            {
                return Visibility.Collapsed;
            }
            else return Visibility.Visible;
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class BoolToVisibilityConverter : IValueConverter
    {
        // This converts the DateTime object to the string to display.
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            Visibility v;
            // Retrieve the format string and use it to format the value.
            bool b = (bool)value;

            if (b)
                v = Visibility.Visible;
            else v = Visibility.Collapsed;

            // If the format string is null or empty, simply call ToString()
            // on the value.
            return v;
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class VisibilityReverseConverter : IValueConverter
    {
        // This converts the DateTime object to the string to display.
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            Visibility v;
            // Retrieve the format string and use it to format the value.
            var b = (Visibility)value;

            if (b == Visibility.Collapsed)
                v = Visibility.Visible;
            else v = Visibility.Collapsed;

            // If the format string is null or empty, simply call ToString()
            // on the value.
            return v;
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
