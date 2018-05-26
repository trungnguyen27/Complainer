using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace ComplainerProd.QR
{
    public static class VideoCaptureDeviceFinder
    {
        public static async Task<DeviceInformation> FindFirstOrDefaultAsync()
        {
            var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            return (devices.FirstOrDefault());
        }
        public static async Task<DeviceInformation> FindAsync(
          Func<DeviceInformationCollection, DeviceInformation> filter)
        {
            var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            return (filter(devices));
        }
    }
}
