using ComplainerProd.Content_Dialogs;
using ComplainerProd.DataObject;
using ComplainerProd.Models;
using ComplainerProd.QR;
using ComplainerProd.UserControls;
using ComplainerProd.Workers;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
using static ComplainerProd.Enumerations;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ComplainerProd.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChannelFeedbackPage : Page
    {
        public static ChannelFeedbackPage instance;

        public ObservableCollection<Feedback> feedbacks = new ObservableCollection<Feedback>();

        public Channel Channel
        {
            get { return (Channel)GetValue(FeedbackProp); }
            set
            {
                SetValueDp(FeedbackProp, value);
            }
        }

        public static readonly DependencyProperty FeedbackProp =
            DependencyProperty.Register("Channel", typeof(Channel), typeof(CommentPage), null);

        public event PropertyChangedEventHandler PropertyChanged;

        void SetValueDp(DependencyProperty property, object value, [System.Runtime.CompilerServices.CallerMemberName] String p = null)
        {
            SetValue(property, value);
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }

        private CommentPage commentPage;

        private ScrollViewer scrollBar; 

        public ChannelFeedbackPage()
        {
            this.InitializeComponent();
            FBFormControl.SendBtn.Click += SendBtn_Click;
            FBFormControl.BackBtn.Click += BackBtn_Click;
            FBFormControl.AddImageBtn.Click += AddImageBtn_Click;
            FBFormControl.ShowPanel.Completed += ShowPanel_Completed;
            CommentFrame.Navigate(typeof(CommentPage), new Feedback());
            commentPage = CommentFrame.Content as CommentPage;
            commentPage.BackButton.Click += BackButton_Click;
            SendFeedbackForm.Completed += SendFeedbackForm_Completed;
            ShowFeedbackForm.Completed += ShowFeedbackForm_Completed;
            HideFeedbackForm.Completed += HideFeedbackForm_Completed;
        }

        private void SendFeedbackForm_Completed(object sender, object e)
        {
            FeedbackFromCT.TranslateY = 800;
            FBFormControl.HidePanel.Begin();
            FilledBackground.IsHitTestVisible = false;
        }

        private void ShowFeedbackForm_Completed(object sender, object e)
        {
            FilledBackground.IsHitTestVisible = true;
        }

        private void ShowPanel_Completed(object sender, object e)
        {
            SendFeedbackForm.Begin();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            HideFeedbackForm.Begin();
            FBFormControl.FeedbackTextBox.Text = "";
        }

        private async void AddImageBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var file = await FilePicker.OpenPicker(new List<string>() { ".jpg", ".png" });
                FBFormControl.SetSupportImageSource(await FilePicker.ConvertToBitmapFromFile(file));
                if (file != null)
                {
                    //var encoded = await FilePicker.EncodeFile(file);
                    FBFormControl.Imagefile = file;
                }
            }
            catch
            {
                await HelpersClass.ShowDialogAsync("Lỗi khi thêm hình ảnh");
            }
            
        }


        private void HideFeedbackForm_Completed(object sender, object e)
        {
            FBFormControl.Visibility = Visibility.Collapsed;
        }

        private async void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = await SendFeedbackAsync();
            if (result)
            {
                FBFormControl.ShowPanel.Begin();
            }
            else
            {
                HideFeedbackForm.Begin();
            }
            FBFormControl.Imagefile = null;
            FBFormControl.SetSupportImageSource(null);
            FBFormControl.FeedbackTextBox.Text = "";
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.Parameter!= null)
            {
                Channel = e.Parameter as Channel;
                await RetrieveFeedback();
               scrollBar = Scrolling(FeedbackListView); 
            }
        }

        private async Task RetrieveFeedback()
        {
            LoadingStackPanel.Visibility = Visibility.Visible;
            feedbacks.Clear();
            feedbacks = new ObservableCollection<Feedback>(await MainPage.SO.GetFeedbacksByChannelAsync(Channel.Id));

            if (feedbacks != null)
            {
                //feedbacks = _feedbacks;
                FeedbackListView.ItemsSource = feedbacks;
                StatusViewBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                StatusViewBox.Visibility = Visibility.Visible;
            }
            LoadingStackPanel.Visibility = Visibility.Collapsed;
        }

        private void AddFeedback_Click(object sender, RoutedEventArgs e)
        {
            ChannelFeedbackGrid.Visibility = Visibility.Collapsed;
            FeedbackCreationGrid.Visibility = Visibility.Visible;
        }

        private async Task<bool> SendFeedbackAsync()
        {
            FBFormControl.SendBtn.IsEnabled = false;
            string id = Guid.NewGuid().ToString();

            string downloadURL;
            if (FBFormControl.Imagefile != null)
            {
                downloadURL = await App.CloudStorage.StoreFileAsync(FBFormControl.Imagefile, id);
            }
            else downloadURL = null;

            Feedback feedback = new Feedback()
            {
                UserID = MainPage.instance.user.UserId,
                ChannelID = Channel.Id,
                Upvote = 0,
                Downvote = 0,
                AttachedImage = downloadURL == null? "" : downloadURL,
                Content = FBFormControl.FeedbackTextBox.Text,
                Action = -1,
                Location = "Unknown" };
            
            var result = await MainPage.SO.AddFeedbackAsync(feedback);
            if (result != null)
            {
                InserAFeedback(0, result);
                // scrollBar.ChangeView(0, 0, 0);
                return true;
            }
            else
            {
                await HelpersClass.ShowDialogAsync("Bạn đã vượt quá 5 phản hồi cho ngày hôm nay :(");
                return false;
            }
            
        }

        private void InserAFeedback(int index, Feedback fb)
        {
            if (feedbacks.Count == 0)
            {
                feedbacks.Add(fb);
            }
            else
                feedbacks.Insert(0, fb);
            FBFormControl.FeedbackTextBox.Text = "";
            StatusViewBox.Visibility = Visibility.Collapsed;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //if (Channel != null)
            //    await RetrieveFeedback();
        }

        private async void DownvoteBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleButton;
            var fbc = btn.DataContext as FeedbackControl;
            var feedback = fbc.Feedback;

            var index = feedbacks.IndexOf(feedback);

            //disable button
            btn.IsEnabled = false;

            //Cast a vote, 1 for upvote,2 for downvote, update if existed
            var result = await MainPage.SO.VoteAsync(MainPage.instance.user.UserId, 0, feedback);
            //Update number
            feedback =  await MainPage.SO.RefreshVotes(feedback);

            feedbacks[index] = feedback;

            try
            {
                (CommentFrame.Content as CommentPage).Feedback = feedback;

            }
            catch (Exception)
            {
                await HelpersClass.ShowDialogAsync("Có lỗi xảy ra");
            }

            //enable button
            btn.IsEnabled = true;
            //fbc.DownvoteBtn.IsChecked = result;
            //fbc.UpvoteBtn.IsChecked = fbc.UpvoteBtn.IsChecked == true ? false : false;
        }

        private async void UpvoteBtn_ClickAsync(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleButton;
            var fbc = btn.DataContext as FeedbackControl;
            var feedback = fbc.Feedback;

            var index = feedbacks.IndexOf(feedback);
            //Disable butn
            btn.IsEnabled = false;

            var result = await MainPage.SO.VoteAsync(MainPage.instance.user.UserId, 1, feedback);
            //Update number
            feedback = await MainPage.SO.RefreshVotes(feedback);

            feedbacks[index] = feedback;

            try
            {
                (CommentFrame.Content as CommentPage).Feedback = feedback;

            }
            catch (Exception ex)
            {

            }

            //Enable Butn
            btn.IsEnabled = true;

            //fbc.UpvoteBtn.IsChecked = result;
            //fbc.DownvoteBtn.IsChecked = fbc.DownvoteBtn.IsChecked == true ? false : false;
        }

        private async void RefeshButton_Click(object sender, RoutedEventArgs e)
        {
            await RetrieveFeedback();
        }

        private void FeedbackControl_Loaded(object sender, RoutedEventArgs e)
        {
            var sd = sender as FeedbackControl;

            sd.UpvoteBtn.Click += UpvoteBtn_ClickAsync;
            sd.DownvoteBtn.Click += DownvoteBtn_Click;
            sd.DataContextChanged += Sd_DataContextChanged;
        }

        private  void Sd_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var sd = sender as FeedbackControl;
            sd.Feedback = sd.DataContext as Feedback;
            if (sd.Feedback == null) return;


        }

        private void FeedbackControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {          
        }

        private void FeedbackListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            
        }

        private async void FeedbackListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FeedbackListView.SelectedIndex == -1)
                return;
            var sd = sender as ListView;
            var fb = sd.SelectedItem as Feedback;
            if (this.Frame.ActualWidth <= App.WIDE_STATE)
            {
                FeedbackClickedTrigger.IsActive = true;
                BackToFeedbackTrigger.IsActive = false;
            }
            else
            {

            }
            await commentPage.SetNewFeedback( fb != null ? fb : new Feedback());
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
                FeedbackListView.SelectedIndex = -1;
                BackToFeedbackTrigger.IsActive = true;
                FeedbackClickedTrigger.IsActive = false;
            }
            else
            {

            }
        }

        private void AddFeedbackButton_Click(object sender, RoutedEventArgs e)
        {
            FBFormControl.Visibility = Visibility.Visible;
            ShowFeedbackForm.Begin();
        }

        private async void CodeTBlock_Click(object sender, RoutedEventArgs e)
        {
            var sd = sender as Button;
            QRCodeViewDialog d = new QRCodeViewDialog();
            d.Channel = Channel;
            d.QRCodeProp.Source = await QRCodeFactory.GetQrCode(this.Channel.AccessCode, "L");
            await d.ShowAsync();
        }

        private void PointerEntered(object sender, PointerRoutedEventArgs e)
        {
           
        }

        private ScrollViewer Scrolling(DependencyObject depObj)
        {
            ScrollViewer foundOne = GetScrollViewer(depObj);
            if (foundOne != null)
            {
                //    if (foundOne.VerticalOffset == 0)
                //    //refresh.Visibility = Visibility.Visible;
                //    else
                //refresh.Visibility = Visibility.Collapsed;
                foundOne.ViewChanging += foundOne_ViewChanging;
            }


            return foundOne;
        }

        public static ScrollViewer GetScrollViewer(DependencyObject depObj)
        {
            if (depObj is ScrollViewer) return depObj as ScrollViewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = GetScrollViewer(child);
                if (result != null) return result;
            }
            return null;
        }

        private void foundOne_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            //YOur logic to hide the button
            if (e.NextView.VerticalOffset > 100)
            {
                FooterGrid.Visibility = Visibility.Visible;
            }
            else 
            {
                FooterGrid.Visibility = Visibility.Collapsed;
            }
        }

        private async void PhoneButton_Click(object sender, RoutedEventArgs e)
        {
            // The URI to launch
            string uri = @"ms-people:savetocontact?PhoneNumber="+ Channel.Phone +"&ContactName="+ Channel.Name;
            await HelpersClass.LaunchAURI(uri);
        }

        private async void URLButton_Click(object sender, RoutedEventArgs e)
        {
            // The URI to launch
            string uri = "";

            if (!Channel.StorageURL.Contains("www"))
            {
                uri = @"https://www." + Channel.StorageURL;
            }else if (!Channel.StorageURL.Contains("http"))
            {
                uri = @"http://" + Channel.StorageURL;
            }


            await HelpersClass.LaunchAURI(uri);
        }
        //Sorting Flyout Item Click
        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var sd = sender as MenuFlyoutItem;
            var selection = Sorting.Like;
            switch(sd.Name)
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
            await RetrieveFeedback();
            RefreshButton.IsEnabled = true;
        }
    }
}
