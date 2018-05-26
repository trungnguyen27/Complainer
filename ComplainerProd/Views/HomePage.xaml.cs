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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;



// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ComplainerProd.Views
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public sealed partial class HomePage : Page
    {
        public static HomePage instance;

        public ObservableCollection<Feedback> feedbacks = new ObservableCollection<Feedback>();

        public ObservableCollection<Channel> ChannelFilter = new ObservableCollection<Channel>();

        private CommentPage commentPage;

        private List<Feedback> allFeedbacks = new List<Feedback>();

        public HomePage()
        {
            this.InitializeComponent();
            instance = this;
            MyFeedbackListView.ItemsSource = feedbacks;
            CommentFrame.Navigate(typeof(CommentPage), new Feedback());
            commentPage = CommentFrame.Content as CommentPage;
            commentPage.BackButton.Click += BackButton_Click;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await RefreshData();
        }

        private async Task RefreshData()
        {
            LoadingStackPanel.Visibility = Visibility.Visible;
            try
            {
                allFeedbacks = await MainPage.SO.GetFeedbackByUserAsync(MainPage.instance.user.UserId);
                allFeedbacks.ForEach(fb => feedbacks.Add(fb));
                if (feedbacks.Count == 0)
                    StatusStackPanel.Visibility = Visibility.Visible;
                else StatusStackPanel.Visibility = Visibility.Collapsed;

                var filters = new Dictionary<int, Channel>();
                foreach (var feedback in feedbacks)
                {
                    if (!filters.ContainsKey(feedback.ChannelID)) filters.Add(feedback.ChannelID, new Channel() { Id = feedback.ChannelID, Name = feedback.ChannelName });
                }
                Logout.IsEnabled = false;
                ChannelFilter.Clear();
                ChannelFilter = new ObservableCollection<Channel>(filters.Values.ToList());

                Logout.IsEnabled = true;
                ChannelFilterComboBox.ItemsSource = ChannelFilter;
                ChannelFilterComboBox.SelectedIndex = 0;
            }
            catch
            {

            }
            LoadingStackPanel.Visibility = Visibility.Collapsed;
        }

        private void ChannelSearchListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            NavigationPage.instance.NavigateTo(typeof(ChannelFeedbackPage));
        }

        private void MyFeedback_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selected = e.ClickedItem as Feedback;
            commentPage.SetNewFeedback(selected);
           
            if (this.Frame.ActualWidth <= App.WIDE_STATE)
            {
                FeedbackClickedTrigger.IsActive = true;
                BackToFeedbackTrigger.IsActive = false;
            }
            else
            {
               
            }
        }

        private void ChannelFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                feedbacks.Clear();
                var item = ((ComboBox)sender).SelectedItem as Channel;
                if (item.Name == "All")
                    allFeedbacks.ForEach(fb => feedbacks.Add(fb));
                else
                {
                    allFeedbacks.ForEach(fb =>
                    {
                        if(fb.ChannelName == item.Name)
                        {
                            feedbacks.Add(fb);
                        }
                    });
                }
            }
            catch (NullReferenceException)
            {
            }
        }

        private void FeedbackControl_Loaded(object sender, RoutedEventArgs e)
        {
            var sd = sender as FeedbackControl;
            sd.UpvoteBtn.Click += UpvoteBtn_Click;
            sd.DownvoteBtn.Click += DownvoteBtn_Click;
        }

        private async void DownvoteBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleButton;
            var fbc = btn.DataContext as FeedbackControl;
            var feedback = fbc.Feedback;
            try
            {
                var index = feedbacks.IndexOf(feedback);

                //disable button
                btn.IsEnabled = false;

                //Cast a vote, 1 for upvote,2 for downvote, update if existed
                var result = await MainPage.SO.VoteAsync(MainPage.instance.user.UserId, 0, feedback);
                //Update number
                feedback = await MainPage.SO.RefreshVotes(feedback);

                feedbacks[index] = feedback;

                if (CommentFrame.Content != null)
                {
                    (CommentFrame.Content as CommentPage).Feedback = feedback;
                }
                btn.IsEnabled = true;
            }
            catch (Exception ex)
            {
                await HelpersClass.ShowDialogAsync("Khi con tim yếu mềm, lỗi xuất hiện: " + ex.Message);
            }
        }

        private async void UpvoteBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleButton;
            var fbc = btn.DataContext as FeedbackControl;
            var feedback = fbc.Feedback;

            try
            {
                var index = feedbacks.IndexOf(feedback);

                //Disable butn
                btn.IsEnabled = false;

                var result = await MainPage.SO.VoteAsync(MainPage.instance.user.UserId, 1, feedback);
                //Update number
                feedback = await MainPage.SO.RefreshVotes(feedback);

                feedbacks[index] = feedback;

                if(CommentFrame.Content != null)
                {
                    (CommentFrame.Content as CommentPage).Feedback = feedback;
                }
                //Enable Butn
                btn.IsEnabled = true;
            }
            catch (Exception ex)
            {
                await HelpersClass.ShowDialogAsync("Khi con tim yếu mềm, lỗi xuất hiện: " + ex.Message);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MyFeedbackListView.SelectedItems.Count == 0) return;
            foreach (Feedback fb in MyFeedbackListView.SelectedItems)
                await MainPage.SO.DeleteFeedbackAsync(fb);
            MyFeedbackListView.SelectionMode = ListViewSelectionMode.Single;
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            var s = ChannelFilterComboBox.SelectedItem as Channel;
            await RefreshData();

        }

        private async void FindChannelTBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            try
            {
                var channel = await MainPage.SO.GetChannelByAccessCode(args.QueryText);
                if (channel!=null)
                {
                    NavigationPage.instance.NavigateTo(typeof(ChannelFeedbackPage), channel);
                }
                else
                {
                    await HelpersClass.ShowDialogAsync("Không tìm thấy kênh");
                }
            }
            catch (Exception e)
            {
                await HelpersClass.ShowDialogAsync("Có lỗi xảy ra");
            }
        }

        private async void Logout_Click(object sender, RoutedEventArgs e)
        {
            await MainPage.SO.LogoutAsync();
            MainPage.instance.Frame.Navigate(typeof(MainPage));
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var newSize = e.NewSize;
            if (Math.Abs(newSize.Width - e.PreviousSize.Width) < 20)
                return;
            var page = CommentFrame.Content as CommentPage;

            if (newSize.Width <= App.WIDE_STATE)
            {
                page.EnableBackButton(true);
            }
            else
            {
                FeedbackClickedTrigger.IsActive = false;
                BackToFeedbackTrigger.IsActive = false;
                page.EnableBackButton(false);
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {

            if (this.Frame.ActualWidth <= App.WIDE_STATE)
            {
                BackToFeedbackTrigger.IsActive = true;
                FeedbackClickedTrigger.IsActive = false;
            }
            else
            {
               
            }
        }
    }
}
