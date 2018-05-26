using ComplainerProd.Models;
using ComplainerProd.DataObject;
using ComplainerProd.Views;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
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
using Windows.UI.Popups;
using System.Net.Http;
using Windows.Networking.PushNotifications;
using ComplainerProd.Test;
using Windows.Storage;
using ComplainerProd.QR;
using Windows.UI.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ComplainerProd
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage instance;

        RegisterClient rl = new RegisterClient();

        public Frame frame;
        // Define a member variable for storing the signed-in user. 
        public MobileServiceUser user;

        public static ServerObject SO = new ServerObject();

        public PushNotificationChannel channel;

        public QRCodeFactory QRFactory = new QRCodeFactory();

        public string ID;

        private bool notificationIntialized = false;

        public MainPage()
        {
            this.InitializeComponent();
            instance = this;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var result = TryLoggingIn();
            if (result)
            {
                LoginStackPanel.Visibility = Visibility.Collapsed;
                LoggingInStatckPanel.Visibility = Visibility.Visible;
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                            () => this.Frame.Navigate(typeof(NavigationPage)));
                if (notificationIntialized == false)
                    notificationIntialized = await MainPage.instance.InitNotificationsAsync();
            }
            else
            {
                LoginStackPanel.Visibility = Visibility.Visible;
                LoggingInStatckPanel.Visibility = Visibility.Collapsed;
            }
        }

        private bool TryLoggingIn()
        {
            try
            {
                Login();
                SO.GetInfo(user);
            }
            catch(Exception e)
            {
                return false;
            }
            if (this.user != null)
                return true;
            return false;
        }

        private void Login()
        {
            this.user = SO.AuthenticateAsync();
            SO.IntializeTables();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(NavigationPage));
        }

        public async Task<bool> InitNotificationsAsync()
        {
            try
            {
                // Get a channel URI from WNS.
                channel = await PushNotificationChannelManager
                   .CreatePushNotificationChannelForApplicationAsync();

                var regId = await RetriveRegistrationIdOrRequestNewOneAsync();

                // Register the channel URI with Notification Hubs.
                //await App.MobileService.GetPush().RegisterAsync(channel.Uri);

                var statusCode = await rl.RegisterAsync(regId, channel.Uri, new List<string>() { user.UserId });
                if (statusCode == System.Net.HttpStatusCode.Gone)
                {
                    var settings = ApplicationData.Current.LocalSettings.Values;
                    settings.Remove("__NHRegistrationId");
                    regId = await RetriveRegistrationIdOrRequestNewOneAsync();
                    statusCode = await rl.RegisterAsync(regId, channel.Uri, new List<string>() { user.UserId });
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        private async Task<string> RetriveRegistrationIdOrRequestNewOneAsync()
        {
            var settings = ApplicationData.Current.LocalSettings.Values;

            if (!settings.ContainsKey("_NHRegistrationId"))
            {
                ID = await App.MobileService
            .InvokeApiAsync<string>("Admin", HttpMethod.Post, new Dictionary<string, string>() { { "handle", channel.Uri } });
                settings.Add("_NHRegistrationId", ID);
            }
            return (string)settings["_NHRegistrationId"];
        }

        private async void FBLoginButton_Click(object sender, RoutedEventArgs e)
        {
            this.user = await MainPage.SO.Login( MobileServiceAuthenticationProvider.Facebook);
            if(user!=null)
            {
                this.Frame.Navigate(typeof(NavigationPage));
            }
            else
            {
                await HelpersClass.ShowDialogAsync("Something went wrong");
            }
        }

        private async void GGLoginButton_Click(object sender, RoutedEventArgs e)
        {
            this.user = await MainPage.SO.Login(MobileServiceAuthenticationProvider.Google);
            if (user != null)
            {
                this.Frame.Navigate(typeof(NavigationPage));
            }
            else
            {
                await HelpersClass.ShowDialogAsync("Something went wrong");
            }
        }

        private async void PolicyButton_Click(object sender, RoutedEventArgs e)
        {
            string link = @"http://complainer.azurewebsites.net/2017/05/08/policy/";
            await HelpersClass.LaunchAURI(link);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int index = IntroFlipView.SelectedIndex;
            IntroFlipView.SelectedIndex = index == IntroFlipView.Items.Count - 1 ? 0 : index+1;
        }
    }
}
