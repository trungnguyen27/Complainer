using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ComplainerProd.Test
{
    class RegisterClient
    {

        public class DeviceRegistration
        {
            public string Platform { get; set; }
            public string Handle { get; set; }
            public string[] Tags { get; set; }
        }
    
        public async Task<HttpStatusCode> RegisterAsync(string regId, string handle, IEnumerable<string> tags)
        {
            var deviceRegistration = new DeviceRegistration
            {
                Platform = "wns",
                Handle = handle,
                Tags = tags.ToArray<string>()
            };

            string json = JsonConvert.SerializeObject(deviceRegistration);

            var response = await App.MobileService
                .InvokeApiAsync<HttpStatusCode>("Admin", HttpMethod.Post, new Dictionary<string, string>() { { "id", regId }, { "json", json } });
            return response;
        }

    }


}
