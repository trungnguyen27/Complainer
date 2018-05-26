using ComplainerProd.DataObject;
using ComplainerProd.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ComplainerProd.UserControls
{
    public sealed partial class ChannelHistoryControl : UserControl
    {
        #region
        public Channel Channel
        {
            get { return (Channel)GetValue(ChannelProp); }
            set
            {
                SetValueDp(ChannelProp, value);
            }
        }

        public ChannelStatistic ChannelStatistic
        {
            get { return (ChannelStatistic)GetValue(ChannelStatisticProp); }
            set
            {
                SetValueDp(ChannelStatisticProp, value);
            }
        }

        public static readonly DependencyProperty ChannelProp =
        DependencyProperty.Register("Channel", typeof(Channel), typeof(ChannelHistoryControl), null);

        public static readonly DependencyProperty ChannelStatisticProp =
        DependencyProperty.Register("ChannelStatistic", typeof(Channel), typeof(ChannelHistoryControl), null);

        public event PropertyChangedEventHandler PropertyChanged;

        void SetValueDp(DependencyProperty property, object value, [System.Runtime.CompilerServices.CallerMemberName] String p = null)
        {
            SetValue(property, value);
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }
        #endregion
        public ChannelHistoryControl()
        {
            this.InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
            AccentBackground.Background = RandomizeBrush();
        }

        private Brush RandomizeBrush()
        {
            List<string> hexes = new List<string>() { "#FFB900", "#E81123", "#E74856", "#0078D7", "#0099BC", "#4C4A48", "#498205", "#E3008C" };

            var colors = typeof(Colors).GetRuntimeProperties();

            Random rand = new Random(DateTimeOffset.Now.Millisecond);
            var index = rand.Next(hexes.Count);

            return HelpersClass.GetSolidColorBrush(hexes[index]);
        }
    }
}
