using ComplainerProd.QR.Processors;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using ZXing;

namespace ComplainerProd.QR
{
    public class QRCodeFactory
    {
        public static async Task<BitmapImage> GetQrCode(string SourceText, string Ecc)
        {
            var level = Ecc;
            var eccLevel = (QRCodeGenerator.ECCLevel)(level == "L" ? 0 : level == "M" ? 1 : level == "Q" ? 2 : 3);
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(SourceText, eccLevel);
            var qrCode = new BitmapByteQRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);

            using (var stream = new InMemoryRandomAccessStream())
            {
                using (var writer = new DataWriter(stream.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(qrCodeImage);
                    await writer.StoreAsync();
                }
                BitmapImage bit = new BitmapImage();
                await bit.SetSourceAsync(stream);
                BitmapImage Bitmap = bit;
                return Bitmap;
            }
        }

        public QrCaptureFrameProcessor frameProcessor = null;

        public async Task<bool> Initialize(MediaCapture mediaCap)
        {
            var mediaFrameSourceFinder = new MediaFrameSourceFinder();

            // We want a source of media frame groups which contains a color video
            // preview (and we'll take the first one).
            var populated = await mediaFrameSourceFinder.PopulateAsync(
              MediaFrameSourceFinder.ColorVideoPreviewFilter,
              MediaFrameSourceFinder.FirstOrDefault);

            if(populated)
            {
                // We'll take the first video capture device.
                var videoCaptureDevice =
                  await VideoCaptureDeviceFinder.FindFirstOrDefaultAsync();
                // Make a processor which will pull frames from the camera and run
                // ZXing over them to look for QR codes.
                frameProcessor = new QrCaptureFrameProcessor(
                  mediaFrameSourceFinder,
                  videoCaptureDevice,
                  MediaEncodingSubtypes.Bgra8,
                  mediaCap);
                return true;
            }
            return false;
        }

        public async Task ScanFirstCameraForQrCode(
                                                Action<Result> resultCallback,
                                                TimeSpan timeout)
        {
            Result result = null;


            // Remember to ask for auto-focus on the video capture device.
            frameProcessor.SetVideoDeviceControllerInitialiser(
              vd => vd.Focus.TrySetAuto(true));

            // Process frames for up to 30 seconds to see if we get any QR codes...
            await frameProcessor.ProcessFramesAsync(timeout);

            // See what result we got.
            result = frameProcessor.QrZxingResult;
          
            // Call back with whatever result we got.
            resultCallback(result);
        }

    }
}
