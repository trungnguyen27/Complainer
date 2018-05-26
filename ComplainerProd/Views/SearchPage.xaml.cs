using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
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
using ComplainerProd.QR;
using Windows.System.Display;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using ComplainerProd.DataObject;
using Windows.Media.Capture;
using Windows.UI.Core;
using Windows.ApplicationModel;
using ComplainerProd.Models;
using ZXing;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using ComplainerProd.Workers;
using System.Collections.ObjectModel;
using System.Net.Http;
using ComplainerProd.UserControls;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ComplainerProd.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchPage : Page
    {

        private Channel _foundChannel = new Channel();

        public SearchPage()
        {
            this.InitializeComponent();
            this.Loaded += SearchPage_Loaded;
            Application.Current.Suspending += Application_Suspending;
        }

        private async Task GetBackgroundImagesAsync()
        {
            try
            {
                var folderPath = @"Assets\Graphics\Backgrounds";
                var backgroundFol = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(folderPath);
                if(backgroundFol!=null)
                {
                    var files =await backgroundFol.GetFilesAsync();
                    Random rand = new Random();
                    int index = rand.Next(files.Count);
                    BackgroundImage.Source =await FilePicker.ConvertToBitmapFromFile(files[index]);
                }
            }catch(UnauthorizedAccessException unauthorizedEx)
            {
                await HelpersClass.ShowDialogAsync("Cannot access to background folder " + unauthorizedEx.Message);
            }catch(Exception ex)
            {
                await HelpersClass.ShowDialogAsync("something went really wrong " + ex.Message);
            }

        }

        private async void SearchPage_Loaded(object sender, RoutedEventArgs e)
        {

            //var _result = await MainPage.SO.UserTable.Select(p => p).ToListAsync();
         
            //var _result = await App.MobileService.InvokeApiAsync("/api/Channels", HttpMethod.Get, null);
            //int a = 55;
            ////var result = await App.MobileService
            //        //.InvokeApiAsync<IList<Channel>>("Admin", HttpMethod.Get, new Dictionary<string, string>() { { "userIDForChannelHistory", MainPage.instance.user.UserId } });


            //var collection = new ObservableCollection<Channel>();

            //if(result!= null)
            //{
            //    if (result.Count == 0)
            //    {
            //        RecentChannelsGrid.Visibility = Visibility.Collapsed;
            //    }
            //    else
            //    {
            //        RecentChannelsGrid.Visibility = Visibility.Visible;
            //    }

            //    foreach (var c in result)
            //    {
            //        collection.Add(c);
            //    }
            //}
            //else
            //{
            //    RecentChannelsGrid.Visibility = Visibility.Collapsed;
            //}
            //RecentChannelListview.ItemsSource = collection;
        }

        private async void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            try
            {
                SearchBox.IsEnabled = false;
                PBar.Visibility = Visibility.Visible;
                StatusTblock.Visibility = Visibility.Collapsed;
                PBar.IsActive = true;

                var channel = await MainPage.SO.GetChannelByAccessCode(SearchBox.Text);
                if(channel != null)
                {
                    NavigationPage.instance.NavigateTo(typeof(ChannelFeedbackPage), channel);
                }
                else
                {
                    StatusTblock.Visibility = Visibility.Visible;
                    StatusTblock.Text = "Không tìm thấy kênh nào...";
                }
                SearchBox.IsEnabled = true;
                PBar.Visibility = Visibility.Collapsed;
                PBar.IsActive = false;
            }
            catch (Exception e)
            {

            }
        }

        private void CreateChannelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationPage.instance.NavigateTo(typeof(ChannelPage), true);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
           SearchBox_QuerySubmitted(SearchBox, new AutoSuggestBoxQuerySubmittedEventArgs());

        }

        private void QRScanButton_Click(object sender, RoutedEventArgs e)
        {
            MobileLayoutTrigger.IsActive = true;
            StartScan();
        }
        #region Testing
        MediaCapture _mediaCapture;
        bool _isPreviewing;
        DisplayRequest _displayRequest = new DisplayRequest();
        private async Task StartPreviewAsync()
        {
            try
            {

                _mediaCapture = new MediaCapture();
                await _mediaCapture.InitializeAsync();

                _displayRequest.RequestActive();
            }
            catch (UnauthorizedAccessException)
            {
                // This will be thrown if the user denied access to the camera in privacy settings
                await HelpersClass.ShowDialogAsync("The app was denied access to the camera");
                return;
            }

            try
            {
                PreviewControl.Source = _mediaCapture;
                await _mediaCapture.StartPreviewAsync();
                _isPreviewing = true;
            }
            catch (System.IO.FileLoadException)
            {
                //_mediaCapture.CaptureDeviceExclusiveControlStatusChanged += _mediaCapture_CaptureDeviceExclusiveControlStatusChanged;
            }

        }
        //private async void _mediaCapture_CaptureDeviceExclusiveControlStatusChanged(MediaCapture sender, MediaCaptureDeviceExclusiveControlStatusChangedEventArgs args)
        //{
        //    if (args.Status == MediaCaptureDeviceExclusiveControlStatus.SharedReadOnlyAvailable)
        //    {
        //        ShowMessageToUser("The camera preview can't be displayed because another app has exclusive access");
        //    }
        //    else if (args.Status == MediaCaptureDeviceExclusiveControlStatus.ExclusiveControlAvailable && !_isPreviewing)
        //    {
        //        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
        //        {
        //            await StartPreviewAsync();
        //        });
        //    }
        //}

        private async Task CleanupCameraAsync()
        {
            if (_mediaCapture != null)
            {
                if (_isPreviewing)
                {
                    await _mediaCapture.StopPreviewAsync();
                }

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    PreviewControl.Source = null;
                    if (_displayRequest != null)
                    {
                        _displayRequest.RequestRelease();
                    }

                    _mediaCapture.Dispose();
                    _mediaCapture = null;
                });
            }
        }
        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            await CleanupCameraAsync();
            if (MainPage.instance.QRFactory.frameProcessor != null)
                MainPage.instance.QRFactory.frameProcessor.Dispose();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            PCLayoutTrigger.IsActive = true;
            if (HelpersClass.GetDeviceFormFactorType() == HelpersClass.DeviceFormFactorType.Desktop)
            {
                //PCLayoutTrigger.IsActive = true;
            }
            else if (HelpersClass.GetDeviceFormFactorType() == HelpersClass.DeviceFormFactorType.Phone)
            {
                QRScanButton.Visibility = Visibility.Visible;
            }

            await GetBackgroundImagesAsync();
        }

        String text = "";
        private async void StartScan()
        {
            await StartPreviewAsync();
            await MainPage.instance.QRFactory.Initialize(_mediaCapture);
            do
            {
                await MainPage.instance.QRFactory.ScanFirstCameraForQrCode(Function, TimeSpan.FromSeconds(2));
            } while (text == "None");
        }

        private async void Function(Result result)
        {
            text = result?.Text ?? "None";
            SubSearchBox.Text = text;
            if (text != "None")
            {
                try
                {
                    var list = await MainPage.SO.ChannelTable.Where(c => c.AccessCode == SearchBox.Text).ToListAsync();
                    if (list.Count >= 1)
                    {
                        ResultTblock.Text = list[0].Name;
                        _foundChannel = list[0];
                        ShowResult.Begin();
                    }
                }
                catch (Exception e)
                {

                }
            }
            await Task.Delay(1000);
        }

        private async void Application_Suspending(object sender, SuspendingEventArgs e)
        {
            // Handle global application events only if this page is active
            if (Frame.CurrentSourcePageType == typeof(MainPage))
            {
                var deferral = e.SuspendingOperation.GetDeferral();
                await CleanupCameraAsync();
                deferral.Complete();
            }
        }
        #endregion

        private void GoToButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationPage.instance.NavigateTo(typeof(ChannelFeedbackPage), _foundChannel);
            HideResult.Begin();
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            BlurBehavior.Value = 7;
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            BlurBehavior.Value = 0;
        }

        private async void ChannelHistoryControl_Loaded(object sender, RoutedEventArgs e)
        {
            var sd = sender as ChannelHistoryControl;
           
            if (sd == null)
                return;
            int id = (sd.DataContext as Channel).Id;
            sd.ChannelStatistic = await MainPage.SO.GetStatistic(id);
        }

        private void RecentChannelListview_ItemClick(object sender, ItemClickEventArgs e)
        {
            var channel = e.ClickedItem as Channel;
            NavigationPage.instance.NavigateTo(typeof(ChannelFeedbackPage), channel);
        }

        
    }
}
