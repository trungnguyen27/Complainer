using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace ComplainerProd
{
    public class Sharing
    {
        public DataTransferManager dataTransferManager;

        private StorageFile ToShareData;

        public Sharing()
        {
            dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
        }

        public void Show(StorageFile file)
        {
            ToShareData = file;
            DataTransferManager.ShowShareUI();
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.Properties.Title = "Distribute Your Code";
            request.Data.Properties.Description = "Let the feedback rain";
            request.Data.SetStorageItems(new List<StorageFile>() { ToShareData });
        }

        async void OnDeferredImageRequestedHandler(DataProviderRequest request, IRandomAccessStream imageStream)
        {
            // Provide updated bitmap data using delayed rendering
            if (imageStream != null)
            {
                DataProviderDeferral deferral = request.GetDeferral();
                InMemoryRandomAccessStream inMemoryStream = new InMemoryRandomAccessStream();

                // Decode the image.
                BitmapDecoder imageDecoder = await BitmapDecoder.CreateAsync(imageStream);

                // Re-encode the image at 50% width and height.
                BitmapEncoder imageEncoder = await BitmapEncoder.CreateForTranscodingAsync(inMemoryStream, imageDecoder);
                imageEncoder.BitmapTransform.ScaledWidth = (uint)(imageDecoder.OrientedPixelHeight * 0.5);
                imageEncoder.BitmapTransform.ScaledHeight = (uint)(imageDecoder.OrientedPixelHeight * 0.5);
                await imageEncoder.FlushAsync();

                request.SetData(RandomAccessStreamReference.CreateFromStream(inMemoryStream));
                deferral.Complete();
            }
        }
    }
}
