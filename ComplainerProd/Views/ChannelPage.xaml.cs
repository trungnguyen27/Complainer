using ComplainerProd.Models;
using ComplainerProd.DataObject;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
using Microsoft.Toolkit.Uwp.UI.Controls;
using ComplainerProd.UserControls;
using System.Net.Http;
using Windows.UI;
using ComplainerProd.Content_Dialogs;
using ComplainerProd.QR;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ComplainerProd.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChannelPage : Page
    {
        public ObservableCollection<Channel> channels = new ObservableCollection<Channel>();

        public static ChannelPage instance;

        public Channel SelectedChannel { get; set; }

        public ChannelPage()
        {
            this.InitializeComponent();
            instance = this;
            ChannelListView.ItemsSource = channels;
            (this.Content as FrameworkElement).DataContext = this;
            ChannelControl.CreateAction = CreateChannelAsync;
            ChannelControl.CancelBtn.Click += CancelBtn_Click;
            ChannelControl.CheckBtn.Click += CheckBtn_Click;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            SetChannelCreationUI(false);
        }

        private async void CheckBtn_Click(object sender, RoutedEventArgs e)
        {
            string msg = "";
            var c = Colors.Green;
            if (await MainPage.SO.CheckAccessCodeValid(ChannelControl.AccessCode))
            {
                msg = "Holy...mã truy cập này đã tồn tại, hãy chọn  mã khác nhé.";
                c = Colors.Red;
            }
            else
            {
                msg = "Tuyệt vời, bạn có thể dùng mã này.";
            }
            ChannelControl.StatusTB.Text = msg;
            ChannelControl.Foreground = new SolidColorBrush(c);
        }

        private void FBListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            CommentBlade.IsOpen = true;
            CommentFrame.Navigate(typeof(CommentPage), e.ClickedItem as Feedback);
        
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                bool b = false;
                if(e.Parameter is bool)
                {
                    b = (bool)e.Parameter;
                }
               
                await InitializePageAsync(b);
            }catch(Exception ex)
            {
                await HelpersClass.ShowDialogAsync(ex.Message);
            }

            base.OnNavigatedTo(e);
        }

        private async Task InitializePageAsync(bool param)
        {
            var list = await GetChannelsAsync();
            if(list.Count ==0 || param == true)
            {
                SetChannelCreationUI(true);
            }
            else
            {
                SetChannelCreationUI(false);
            }
        }

        private async Task<List<Channel>> GetChannelsAsync()
        {
            try
            {
                var list = await MainPage.SO.GetChannelAsync(MainPage.instance.user.UserId);
                return list.ToList();
            }
            catch (Exception e)
            {
                await HelpersClass.ShowDialogAsync("Something wrong retrieving Channels" + e.Message);
            }
            return null;
        }

        private void ChannelListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            FeedbackBlade.IsOpen = true;
            FeedbackFrame.Navigate(typeof(FeedbackPage), e.ClickedItem as Channel);
            (FeedbackFrame.Content as FeedbackPage).FBListView.ItemClick += FBListView_ItemClick;
        }

        private async Task CreateChannelAsync()
        {
            ChannelControl.CreateChannelButton.IsEnabled = false;
            var c = ChannelControl;
            var channel = new Channel()
            {
                Name = c.ChannelName,
                Location = c.Location,
                AccessCode = c.AccessCode,
                UserId = MainPage.instance.user.UserId,
                StorageURL = c.Web,
                Phone = c.Phone
            };

            var result = await MainPage.SO.AddChannelAsync(channel);

            if(result)
            {
                channels.Clear();
                channels.Add(channel);
                SetChannelCreationUI(false);
            }
            else
            {
                await HelpersClass.ShowDialogAsync("Kiểm tra lại thông tin nhập.");
            }

            
        }

        private void CancelChannelCreation()
        {
            SetChannelCreationUI(false);
        }

        private void AddChannelButton_Click(object sender, RoutedEventArgs e)
        {
            SetChannelCreationUI(true);
        }

        private void EditChannelsTButton_Click(object sender, RoutedEventArgs e)
        {
            var sd = sender as ToggleButton;
            if(sd.IsChecked == true)
            {
                ChannelListView.SelectionMode = ListViewSelectionMode.Multiple;
                AddChannelButton.Visibility = Visibility.Collapsed;
                DeleteChannelsButton.Visibility = Visibility.Visible;
            }else
            {
                ChannelListView.SelectionMode = ListViewSelectionMode.None;
                AddChannelButton.Visibility = Visibility.Visible;
                DeleteChannelsButton.Visibility = Visibility.Collapsed;
            }

        }

        private async void SetChannelCreationUI(bool s)
        {
            if (!s)
            {
                ChannelCreationGrid.Visibility = Visibility.Collapsed;
                ChannelListView.Visibility = Visibility.Visible;
                AddChannelButton.Visibility = Visibility.Visible;
                channels.Clear();
                var list = await GetChannelsAsync();
                list.ForEach(l => channels.Add(l));
            }
            else
            {
                ChannelCreationGrid.Visibility = Visibility.Visible;
                ChannelListView.Visibility = Visibility.Collapsed;
                AddChannelButton.Visibility = Visibility.Collapsed;
            }
        }



        private async void DeleteChannelsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach(Channel c in ChannelListView.SelectedItems)
                {
                    bool success = await MainPage.SO.DeleteChannel(c);
                    if(success) channels.Remove(c);
                }
            }catch(Exception ex)
            {
                await HelpersClass.ShowDialogAsync("Can't delete: "+ex.Message);
            }
            EditChannelsTButton.IsChecked = false;
            EditChannelsTButton_Click(EditChannelsTButton, new RoutedEventArgs());
        }

        private void ChannelListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sd = sender as ListView;
            if (sd.SelectedItems.Count == 0)
                DeleteChannelsButton.IsEnabled = false;
            else DeleteChannelsButton.IsEnabled = true;
        }

        private async void Expander_Expanded(object sender, EventArgs e)
        {
            var sd = sender as Expander;
            var cc = sd.FindName("CControl") as ChannelControl;
            if (cc== null)
                return;
            int id = (cc.DataContext as Channel).Id;
            cc.ChannelStatistic =await MainPage.SO.GetStatistic(id);
        }


        private void CControl_Loaded(object sender, RoutedEventArgs e)
        {
            var sd = sender as ChannelControl;
            sd.UpdateBtn.Click += UpdateBtn_Click;
            sd.QRBtn.Click += QRBtn_Click;
            sd.AddSubChannelBtn.Click += AddSubChannelBtn_Click;
        }

        private async void AddSubChannelBtn_Click(object sender, RoutedEventArgs e)
        {
            var sd = sender as Button;
            var channelControl = sd.DataContext as ChannelControl;
            var dialog = new SubChannelCreationDialog();
            dialog.AddBtn.Click += AddBtn_Click;
            dialog.Channel = channelControl.Channel;
            await dialog.ShowAsync();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var sd = sender as Button;
            var dialog = sd.DataContext as SubChannelCreationDialog;

            //MainPage.SO.AddSubChannelAsync(dialog.Channel.Id, dialog.SubChannelTB.Text);
        }

        private async void QRBtn_Click(object sender, RoutedEventArgs e)
        {
            var sd = sender as Button;
            var cc = sd.DataContext as ChannelControl;
            QRCodeViewDialog d = new QRCodeViewDialog();
            d.Channel = cc.Channel;
            d.QRCodeProp.Source = await QRCodeFactory.GetQrCode(cc.Channel.AccessCode, "L");
            await d.ShowAsync();
        }

        private async void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sd = sender as Button;
                var cc = sd.DataContext as ChannelControl;
                var updated = cc.Channel;
                var success = await MainPage.SO.UpdateChannel(updated);
                cc.EditBtn.IsChecked = false;
            }
            catch
            {

            }
        }
    }
}
