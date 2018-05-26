using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using static ZXing.RGBLuminanceSource;
using global::ZXing;
using ComplainerProd.DataObject;

namespace ComplainerProd.QR.Processors
{
    public class QrCaptureFrameProcessor : MediaCaptureFrameProcessor
    {
        public Result QrZxingResult { get; private set; }

        public QrCaptureFrameProcessor(
          MediaFrameSourceFinder mediaFrameSourceFinder,
          DeviceInformation videoDeviceInformation,
          string mediaEncodingSubtype,
          MediaCapture mediaCap,
          MediaCaptureMemoryPreference memoryPreference = MediaCaptureMemoryPreference.Cpu)

          : base(
              mediaFrameSourceFinder,
              videoDeviceInformation,
              mediaEncodingSubtype,
              mediaCap,
              memoryPreference)
        {
        }
        protected override bool ProcessFrame(MediaFrameReference frameReference)
        {
            // doc here https://msdn.microsoft.com/en-us/library/windows/apps/xaml/windows.media.capture.frames.videomediaframe.aspx
            // says to dispose this softwarebitmap if you access it.
            try
            {
                using (var bitmap = frameReference.VideoMediaFrame.SoftwareBitmap)
                {
                    try
                    {
                        // I can go via this route or I can go via the IMemoryBufferByteAccess
                        // route but, either way, I seem to end up copying the bytes into
                        // another buffer which I don't really want to do :-(
                        if (this.buffer == null)
                        {
                            this.buffer = new byte[4 * bitmap.PixelHeight * bitmap.PixelWidth];
                        }
                        bitmap.CopyToBuffer(buffer.AsBuffer());

                        this.QrZxingResult = ZXingQRCodeDecoder.DecodeBufferToQRCode(
                          buffer, bitmap.PixelWidth, bitmap.PixelHeight, BitmapFormat.BGR32);
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
                return false;
            }
           
            return (this.QrZxingResult != null);
        }
        byte[] buffer = null;
    }
}
