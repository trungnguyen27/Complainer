using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;

namespace ComplainerProd.Test
{
    class Notification
    {
        private async Task sendPush(string pns, string userTag, string message)
        {
            
            using (var httpClient = new HttpClient())
            {
              
                try
                {
                    await App.MobileService
                     .InvokeApiAsync<Task>("Notifications", HttpMethod.Post, new Dictionary<string, string>() { { "pns", pns }, { "userTag", userTag }, { "message", message } });
                }
                catch (Exception ex)
                {
                    MessageDialog alert = new MessageDialog(ex.Message, "Failed to send " + pns + " message");
                    await alert.ShowAsync();
                }
            }
        }
    }
}
