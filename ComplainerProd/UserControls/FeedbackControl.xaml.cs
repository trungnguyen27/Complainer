using ComplainerProd.Models;
using ComplainerProd.Views;
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

    public sealed partial class FeedbackControl : UserControl
    {

        #region


        public Feedback Feedback
        {
            get { return (Feedback)GetValue(FeedbackProp); }
            set
            {
                SetValueDp(FeedbackProp, value);
            }
        }
       
        public static readonly DependencyProperty FeedbackProp =
    DependencyProperty.Register("Feedback", typeof(Feedback), typeof(FeedbackControl), null);


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

        public ToggleButton UpvoteBtn
        {
            get
            {
                return UpvoteButton;
            }
        }
        public ToggleButton DownvoteBtn
        {
            get
            {
                return DownvoteButton;
            }
        }
        public FeedbackControl()
        {
            this.InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
            //this.DataContextChanged += (s, e) => this.Bindings.Update();
        }

        private void FBControlUC_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            //Feedback = this.DataContext as Feedback;
        }
    }
}
