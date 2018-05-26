using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;
using System.Net.Http;
using ComplainerProd.DataObject;
using System.Threading;

namespace ComplainerProd.Workers
{
    public class CloudUpload
    {
        private FirebaseStorage server = new FirebaseStorage("complainer-165503.appspot.com");

        HttpClient client = new HttpClient();

        Task<byte[]> downloadTask = null;


        public async Task<string> StoreFileAsync(StorageFile file, string name)
        {
            var stream = await file.OpenStreamForReadAsync();

            var task = server.Child(name).PutAsync(stream);

            //task.Progress.ProgressChanged += (s, f) => uploadProgress(f.Percentage);

            var downloadurl = await task;
            return downloadurl;
            //Debug.WriteLine("DOWNLOAD_URL " + downloadurl);
        }
        CancellationTokenSource cts = new CancellationTokenSource();
        
        public async Task<StorageFile> GetFileAsync(string name, string downloadURL)
        {
            StorageFolder localFolder = ApplicationData.Current.TemporaryFolder;
            StorageFile file = await localFolder.CreateFileAsync("Temp",
                CreationCollisionOption.ReplaceExisting);
            
            try
            {
                client.CancelPendingRequests();
                // Create HttpClien
                downloadTask = client.GetByteArrayAsync(downloadURL);
                byte[] buffer = await downloadTask.AsAsyncOperation(); // Download file
                using (Stream stream = await file.OpenStreamForWriteAsync())
                    stream.Write(buffer, 0, buffer.Length); // Save
            }
            catch
            {
                file = null;
            }
            return file;
        }

        public void CancelDownload()
        {
            if(downloadTask != null)
            {
                downloadTask.AsAsyncOperation().Cancel();
                downloadTask = null;
            }
        }
    }
}
