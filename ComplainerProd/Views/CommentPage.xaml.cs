using ComplainerProd.DataObject;
using ComplainerProd.Models;
using ComplainerProd.Workers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ComplainerProd.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    public sealed partial class CommentPage : Page
    {

        ObservableCollection<Comment> comments = new ObservableCollection<Comment>();

        public Feedback Feedback
        {
            get { return (Feedback)GetValue(FeedbackProp); }
            set
            {
                if (value.ChannelName != "")
                    CommandBar.Visibility = Visibility.Visible;
                else CommandBar.Visibility = Visibility.Collapsed;
                SetValueDp(FeedbackProp, value);
            }
        }

        public static readonly DependencyProperty FeedbackProp =
            DependencyProperty.Register("Feedback", typeof(Feedback), typeof(CommentPage), null);

        public event PropertyChangedEventHandler PropertyChanged;

        void SetValueDp(DependencyProperty property, object value, [System.Runtime.CompilerServices.CallerMemberName] String p = null)
        {
            SetValue(property, value);
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }

        public Button BackButton
        {
            get
            {
                return CommentPageBackButton;
            }
        }

        public CommentPage()
        {
            this.InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
            CommentListView.ItemsSource = comments;
        }

        public async Task GetComments(Feedback fb)
        {
            try
            {
                comments.Clear();
                if (fb.Deleted == true) return;

                var _comments = await MainPage.SO.GetCommentsByFeedback(fb);

                _comments.ToList().ForEach(c => comments.Add(c));

                //l.OrderByDescending(a => a.CreatedAt);

                if(comments.Count == 0)
                {
                    NoCommentTB.Visibility = Visibility.Visible;
                }else
                {
                    NoCommentTB.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex) { }
        }

        public void EnableBackButton(bool b)
        {
            if (b) CommentPageBackButton.Visibility = Visibility.Visible;
            else CommentPageBackButton.Visibility = Visibility.Collapsed;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var fb = e.Parameter as Feedback;
            await SetNewFeedback(fb);
        }

        Task<BitmapImage> downloadTask;

        public async Task SetNewFeedback(Feedback fb)
        {
            try
            {
                ImageLoadingStackPanel.Visibility = Visibility.Collapsed;

                if(downloadTask != null)
                {
                    if (!downloadTask.IsCanceled)
                    {
                        App.CloudStorage.CancelDownload();
                        downloadTask.AsAsyncOperation().Cancel();
                    }
                    
                }
                SupportImage.Source = null;
                Feedback = fb;
                await GetComments(Feedback);

                downloadTask = LoadImage(fb);

                SupportImage.Source = await downloadTask;
            }
            catch(Exception e)
            {
                await HelpersClass.ShowDialogAsync("Something went wrong " + e.Message);
            }
        }

        private async Task<BitmapImage> LoadImage(Feedback fb)
        {
            //Load the image
            ImageLoadingStackPanel.Visibility = Visibility.Visible;
            BitmapImage image = null;
            if (fb.AttachedImage != null)
            {
                var file = await App.CloudStorage.GetFileAsync(fb.Id, fb.AttachedImage);
                if (file != null)
                {
                    image = await FilePicker.ConvertToBitmapFromFile(file);
                    Debug.WriteLine(fb.Id + " download completed");
                }else
                {
                    Debug.WriteLine(fb.Id + " download cancelled");
                }
                    
            }
            ImageLoadingStackPanel.Visibility = Visibility.Collapsed;
            return image;
        }

        private void CommentListView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private async void Post_Click(object sender, RoutedEventArgs e)
        {
            Comment cmt = new Comment() { UserID = MainPage.instance.user.UserId, FeedbackID = Feedback.Id, CommentContent = CommentTextBox.Text };
            var res = await MainPage.SO.AddComment(cmt);
            if(res == null)
            {
                await HelpersClass.ShowDialogAsync("You have exceed 10 maximum comments for a day");
            }
            else
            {
                comments.Insert(0, res);
                NoCommentTB.Visibility = Visibility.Collapsed;
            }
            CommentTextBox.Text = "";
        }

        private void MoreButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CommentPg_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            //var sd = sender as CommentPage;
            //sd.Feedback = sd.DataContext as Feedback;
            //if (sd.Feedback == null) return;
        }

        private async void GoToChannelBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var channel = await MainPage.SO.GetChannelById(Feedback.ChannelID);
                NavigationPage.instance.NavigateTo(typeof(ChannelFeedbackPage), channel);
            }
            catch
            {
                await HelpersClass.ShowDialogAsync("Có lỗi xảy ra :( ");
            }

        }

        private void CommentTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                Post_Click(Post, new RoutedEventArgs());
                e.Handled = true;
            }
            
        }

        private void CommentTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = CommentTextBox.Text;
            if (text != "")
            {
                Post.IsEnabled = true;
            }
            else Post.IsEnabled = false;
        }

        private async void SpamReportBtn_Click(object sender, RoutedEventArgs e)
        {
            var sd = sender as MenuFlyoutItem;
            var comment = sd.DataContext as Comment;
            if(comment != null)
            {
                var res= await MainPage.SO.ReportSpamForComment(comment.id, MainPage.instance.user.UserId);
                if(res)
                {
                    await HelpersClass.ShowDialogAsync("Cám ơn bạn đã thông báo chúng tôi");
                }
                else
                {
                    await HelpersClass.ShowDialogAsync("Bạn đã báo cáo bình luận này rồi");
                }
            }
            //MainPage.SO.ReportSpamForComment()
        }
    }
}
