using ComplainerProd.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class ChannelControl : UserControl
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
        DependencyProperty.Register("Channel", typeof(Channel), typeof(ChannelControl), null);

        public static readonly DependencyProperty ChannelStatisticProp =
        DependencyProperty.Register("ChannelStatistic", typeof(Channel), typeof(ChannelControl), null);

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

        public ToggleButton EditBtn
        {
            get
            {
                return EditButton;
            }
        }

        public Button UpdateBtn
        {
            get
            {
                return UpdateButton;
            }
        }


        public Button QRBtn
        {
            get
            {
                return QRButton;
            }
        }

        public Button AddSubChannelBtn
        {
            get
            {
                return AddSubChannelButton;
            }
        }

        public ChannelControl()
        {
            this.InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
        }

    }
}
