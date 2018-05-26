using ComplainerProd.DataObject;
using ComplainerProd.Models;
using ComplainerProd.UserControls;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static ComplainerProd.Enumerations;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ComplainerProd.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FeedbackPage : Page
    {
        public ObservableCollection<Feedback> feedbacks = new ObservableCollection<Feedback>();

        public ListView FBListView
        {
            get
            {
                return FeedbackListView;
            }
        }

        public int Count
        {
            get
            {
                return feedbacks.Count;
            }
        }

        double viewheight;
        private Channel _currentChannel { get; set; }
        public FeedbackPage()
        {
            this.InitializeComponent();
            SubGrid.Loaded += SubGrid_Loaded;
            FeedbackListView.ItemsSource = feedbacks;
        }



        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var chan = e.Parameter as Channel;
            if(chan!=null)
            {
                _currentChannel = chan;
                await GetFeedbacksAsync(chan);
            }
        }

        private async Task GetFeedbacksAsync(Channel c)
        {
            ProgressRing.IsActive = true;
            try
            {
                if(feedbacks.Count == 0)
                {
                    ShowStatus("No Feedback");
                    var results = await MainPage.SO.GetFeedbacksByChannelAsync(c.Id);
                    results.ToList().OrderByDescending(x => x.Upvote).ToList().ForEach(l => feedbacks.Add(l));
                }
                else
                {
                    StatusTBlock.Visibility = Visibility.Collapsed;
                }
            }catch(Exception e)
            {
                ShowStatus(e.Message);
            }
            ProgressRing.IsActive = false;
        }

        private void SubGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var scrollViewer = HelpersClass.FindChildOfType<ScrollViewer>(SubGrid);
            viewheight = SubGrid.ActualHeight;
        }

        private void ShowStatus(string status)
        {
            StatusTBlock.Visibility = Visibility.Visible;
            StatusTBlock.Text = status;
        }

        private void FeedbackControl_Loaded(object sender, RoutedEventArgs e)
        {
            var sd = sender as AdminFeedbackControl;
            sd.ApplyBtn.Click += ApplyBtn_Click;
        }

        private async void ApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            var sd = sender as Button;
            var control = sd.DataContext as AdminFeedbackControl;
            var fb = control.DataContext as Feedback;

            control.ActionApplying = true; 

            fb.Action = control.SelectedIndex;
            if (await MainPage.SO.ActionChange(fb) == false)
                await HelpersClass.ShowDialogAsync("Something went wrong");
            control.ActionApplying = false;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditButton.IsChecked == true)
            {
                FeedbackListView.SelectionMode = ListViewSelectionMode.Multiple;
            }
            else
            {
                FeedbackListView.SelectionMode = ListViewSelectionMode.Single;
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (FeedbackListView.SelectedItems.Count == 0) return;

            foreach(Feedback fb in FeedbackListView.SelectedItems)
            {
                var success = await MainPage.SO.DeleteFeedbackAsync(fb);
                if (success)
                {
                    feedbacks.Remove(fb);
                }
            }
            EditButton.IsChecked = false;
            FeedbackListView.SelectionMode = ListViewSelectionMode.Single;
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var sd = sender as MenuFlyoutItem;
            var selection = Sorting.Like;
            switch (sd.Name)
            {
                case "LikeFlyout":
                    selection = Sorting.Like;
                    break;
                case "CommentFlyout":
                    selection = Sorting.Comment;
                    break;
                case "RecentFlyout":
                    selection = Sorting.Recent;
                    break;
            }
            var list = HelpersClass.ChangeSorting(feedbacks, selection);
            feedbacks.Clear();
            list.ForEach(l => feedbacks.Add(l));
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshButton.IsEnabled = false;
            await GetFeedbacksAsync(_currentChannel??new Channel());
            RefreshButton.IsEnabled = true;
        }
    }
}
