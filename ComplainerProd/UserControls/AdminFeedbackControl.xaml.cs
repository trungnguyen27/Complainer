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
    public sealed partial class AdminFeedbackControl : UserControl
    {
        public AdminFeedbackControl()
        {
            this.InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
        }
        #region
        public string FBContent
        {
            get { return (string)GetValue(FBContentProp); }
            set
            {
                SetValueDp(FBContentProp, value);
            }
        }

        public Feedback Feedback
        {
            get { return (Feedback)GetValue(FeedbackProp); }
            set
            {
                SetValueDp(FeedbackProp, value);
            }
        }

        public int SelectedIndex
        {
            get { return (int)GetValue(selectedIndexProp); }
            set
            {
                SetValueDp(selectedIndexProp, value);
            }
        }

        public static readonly DependencyProperty FBContentProp =
         DependencyProperty.Register("ChannelName", typeof(string), typeof(AdminFeedbackControl), null);

        public static readonly DependencyProperty FeedbackProp =
        DependencyProperty.Register("Feedback", typeof(Feedback), typeof(AdminFeedbackControl), null);

        public static readonly DependencyProperty selectedIndexProp =
        DependencyProperty.Register("SelectedIndex", typeof(int), typeof(AdminFeedbackControl), null);

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

        public Button ApplyBtn
        {
            get
            {
                return ApplyButton;
            }
        }

        public ComboBox ActionCBox
        {
            get
            {
                return ActionComboBox;
            }
        }
        //public ToggleButton DownvoteBtn
        //{
        //    get
        //    {
        //        return DownvoteButton;
        //    }
        //}
        Func<Task> ApplyAction { get; set; }
        private ICommand applyCommand;
        public ICommand ApplyCommand
        {
            get
            {
                return applyCommand ?? (applyCommand = new CommandHandler(() => ApplyAction(), true));
            }
        }

        public bool ActionApplying
        {
            get
            {
                return ProgressRing.IsActive;
            }
            set
            {
                ProgressRing.IsActive = value;
                if (value)
                {
                    ApplyBtn.IsEnabled = false;
                }
                else ApplyBtn.Content = "Applied";
            }
        }

        private void ActionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sd = sender as ComboBox;
            var fb = this.DataContext as Feedback;
            if (fb == null) return;
            if (sd.SelectedIndex != fb.Action)
            {
                ApplyBtn.Content = "Apply";
                ApplyBtn.Visibility = Visibility.Visible;
                ApplyBtn.IsEnabled = true;
            }
            else
            {
                ApplyBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void AdminFBUC_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
        }

        //Func<Task> DownvoteAction { get; set; }
        //private ICommand downvoteCommand;
        //public ICommand DownvoteCommand
        //{
        //    get
        //    {
        //        return downvoteCommand ?? (downvoteCommand = new CommandHandler(() => DownvoteAction(), true));
        //    }
        //}
    }
}
