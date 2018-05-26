using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace ComplainerProd.Workers
{
    public class FilePicker
    {
        public static async Task<StorageFile> OpenPicker(IList<string> filters) 
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            foreach(string fil in filters)
            {
                    picker.FileTypeFilter.Add(fil);
            }

            StorageFile file = await picker.PickSingleFileAsync();
            return file;
        }

        public static async Task<string> EncodeFile(StorageFile file)
        {
            BitmapImage img = new BitmapImage();
            
            using (var stream = await file.OpenAsync(FileAccessMode.Read))
            {
                var bytes = new byte[stream.Size];
                var buffer = await stream.ReadAsync(bytes.AsBuffer(), (uint)stream.Size, InputStreamOptions.None);
                bytes = buffer.ToArray();
                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(bytes);
                return base64String;
            }
        }

        public static async Task<StorageFile> DecodeFile(string base64String, string name)
        {
            StorageFolder localFolder = ApplicationData.Current.TemporaryFolder;
            StorageFile file = await localFolder.CreateFileAsync(name,
                CreationCollisionOption.ReplaceExisting);
            using (var stream = await file.OpenStreamForWriteAsync())
            {
                await stream.CopyToAsync(new MemoryStream(Convert.FromBase64String(base64String)));
            }
            
            return file;
        }

        public static async Task<BitmapImage> ConvertToBitmapFromFile(StorageFile file)
        {
            var stream = await file.OpenAsync(FileAccessMode.Read);
            var bitmap = new BitmapImage();
            await bitmap.SetSourceAsync(stream);
            return bitmap;
        }
    }
}
