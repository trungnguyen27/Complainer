using ComplainerProd.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ComplainerProd.UserControls
{
    public sealed partial class FeedbackFormControl : UserControl
    {
        public Func<Task> SendAction { get; set; }

        public TextBox FeedbackTextBox
        {
            get
            {
                return FeedbackTBox;
            }
        }
        public Button SendBtn
        {
            get
            {
                return SendButton;
            }
        }

        public Button BackBtn
        {
            get
            {
                return CancelButton;
            }
        }

        public Button AddImageBtn
        {
            get
            {
                return AddImageButton;
            }
        }

        public Storyboard ShowPanel
        {
            get
            {
                return ShowThankyouPanel;
            }
        }

        public Storyboard HidePanel
        {
            get
            {
                return HideThankyouPanel;
            }
        }

        public FeedbackFormControl()
        {
            this.InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
        }

        public void SetSupportImageSource(BitmapImage img)
        {
            if (img != null)
                SupportImage.Source = img;
        }

        public StorageFile Imagefile
        {
            get;set;
        }

        private void FeedbackTBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (FeedbackTBox.Text == "")
                SendButton.IsEnabled = false;
            else SendButton.IsEnabled = true;
        }
    }
}
