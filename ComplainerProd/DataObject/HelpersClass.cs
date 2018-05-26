using ComplainerProd.Models;
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Profile;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using static ComplainerProd.Enumerations;

namespace ComplainerProd.DataObject
{
    public class HelpersClass
    {
        public static async Task ShowDialogAsync(string msg)
        {
            //Show dialog
            var dialog = new MessageDialog(msg);
            dialog.Commands.Add(new UICommand("OK"));
            await dialog.ShowAsync();
        }

        public static T FindChildOfType<T>(DependencyObject root) where T : class
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                DependencyObject current = queue.Dequeue();
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(current); i++)
                {
                    var child = VisualTreeHelper.GetChild(current, i);
                    var typedChild = child as T;
                    if (typedChild != null)
                    {
                        return typedChild;
                    }
                    queue.Enqueue(child);
                }
            }
            return null;
        }

        public static DeviceFormFactorType GetDeviceFormFactorType()
        {
            switch (AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                case "Windows.Mobile":
                    return DeviceFormFactorType.Phone;
                case "Windows.Desktop":
                    return UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Mouse
                        ? DeviceFormFactorType.Desktop
                        : DeviceFormFactorType.Tablet;
                case "Windows.Universal":
                    return DeviceFormFactorType.IoT;
                case "Windows.Team":
                    return DeviceFormFactorType.SurfaceHub;
                default:
                    return DeviceFormFactorType.Other;
            }
        }

        public async static Task<bool> LaunchAURI(string link)
        {
            // 
            // The URI to launch
            var uriBing = new Uri(link);

            // Launch the URI
            var success = await Windows.System.Launcher.LaunchUriAsync(uriBing);

            if (success)
            {
                // URI launched
                return true;
            }
            else
            {
                // URI launch failed
                return false;
            }
        }

        public enum DeviceFormFactorType
        {
            Phone,
            Desktop,
            Tablet,
            IoT,
            SurfaceHub,
            Other
        }

        public static List<Feedback> ChangeSorting(IList<Feedback> fbs, Sorting sort)
        {
            var ordered = new List<Feedback>();
            switch (sort)
            {
                case Sorting.Like:
                    ordered = fbs.OrderByDescending(fb => fb.Upvote).ToList();
                    break;
                case Sorting.Comment:
                    ordered = fbs.OrderByDescending(fb => fb.CommentCount).ToList();
                    break;
                case Sorting.Recent:
                    ordered = fbs.OrderByDescending(fb => fb.CreatedAt).ToList();
                    break;
            }
            return ordered;
        }
        public static SolidColorBrush GetSolidColorBrush(string color)
        {
            color = color.Replace("#", "");
            if (color.Length == 6)
            {
                return new SolidColorBrush(Color.FromArgb(255,
                    byte.Parse(color.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(color.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(color.Substring(4, 2), System.Globalization.NumberStyles.HexNumber)));
            }
            else
            {
                return null;
            }
        }
    }
}
