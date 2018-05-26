using ComplainerProd.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    public sealed partial class ChannelCreationControl : UserControl
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

        public string ChannelName
        {
            get { return (string)GetValue(NameProp); }
            set
            {
                SetValueDp(NameProp, value);
            }
        }
        public static readonly DependencyProperty NameProp =
          DependencyProperty.Register("Name", typeof(string), typeof(ChannelCreationControl), null);

        public string AccessCode
        {
            get { return (string)GetValue(AccessCodeProp); }
            set
            {
                SetValueDp(AccessCodeProp, value);
            }
        }
        public static readonly DependencyProperty AccessCodeProp =
          DependencyProperty.Register("AccessCode", typeof(string), typeof(ChannelCreationControl), null);

        public string Location
        {
            get { return (string)GetValue(LocationProp); }
            set
            {
                SetValueDp(LocationProp, value);
            }
        }
        public static readonly DependencyProperty LocationProp =
          DependencyProperty.Register("Location", typeof(string), typeof(ChannelCreationControl), null);

        public string Phone
        {
            get { return (string)GetValue(PhoneProp); }
            set
            {
                SetValueDp(PhoneProp, value);
            }
        }
        public static readonly DependencyProperty PhoneProp =
          DependencyProperty.Register("PhoneProp", typeof(string), typeof(ChannelCreationControl), null);

        public string Web
        {
            get { return (string)GetValue(WebsiteProp); }
            set
            {
                SetValueDp(WebsiteProp, value);
            }
        }
        public static readonly DependencyProperty WebsiteProp =
          DependencyProperty.Register("Website", typeof(string), typeof(ChannelCreationControl), null);

        public static readonly DependencyProperty ChannelProp =
          DependencyProperty.Register("Channel", typeof(Channel), typeof(ChannelCreationControl), null);

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

      
        public Func<Task> CreateAction { get; set; }
        private ICommand createButtonCommand;
        public ICommand CreateButtonCommand
        {
            get
            {
                return createButtonCommand ?? (createButtonCommand = new CommandHandler(() => CreateAction(), true));
            }
        }

   
        public Button CreateChannelButton
        {
            get
            {
                return CreateButton;
            }
        }

        public Button CancelBtn
        {
            get
            {
                return CancelButton;
            }
        }

        public Button CheckBtn
        {
            get
            {
                return CheckButton;
            }
        }

        public TextBlock StatusTB
        {
            get
            {
                return StatusTextBlock;
            }
        }

        public ChannelCreationControl()
        {
            this.InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
        }

        private async void MoreInfoButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog("Đây là mã truy cập nhanh mà khách hàng hoặc người dùng sẽ dùng để truy cập vào kênh phản hồi của bạn\n" +
                "Lưu ý: mã này là duy nhất và không được thay đổi!");
            dialog.Commands.Add(new UICommand("Đóng"));
            await dialog.ShowAsync();
        }

        private void AddInfoButton_Click(object sender, RoutedEventArgs e)
        {
            MoreInfoStackPanel.Visibility = Visibility.Visible;
            AddInfoButton.Visibility = Visibility.Collapsed;
        }
    }
}
