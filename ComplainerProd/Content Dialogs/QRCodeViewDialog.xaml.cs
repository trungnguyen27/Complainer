using ComplainerProd.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ComplainerProd.Content_Dialogs
{
    public sealed partial class QRCodeViewDialog : ContentDialog
    {


        public Channel Channel
        {
            get { return (Channel)GetValue(ChannelProp); }
            set
            {
                SetValueDp(ChannelProp, value);
            }
        }

        public Image QRCodeProp
        {
            get
            {
                return QRCode;
            }
        }

        public static readonly DependencyProperty ChannelProp =
        DependencyProperty.Register("Channel", typeof(Channel), typeof(QRCodeViewDialog), null);

        public event PropertyChangedEventHandler PropertyChanged;

        void SetValueDp(DependencyProperty property, object value, [System.Runtime.CompilerServices.CallerMemberName] String p = null)
        {
            SetValue(property, value);
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }

        public QRCodeViewDialog()
        {
            this.InitializeComponent();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var stream = await QRGrid.RenderToRandomAccessStream();
            Sharing share = new Sharing();
            share.Show(await Save(stream));
        }

        private void D_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
           
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private async Task<StorageFile> Save(IRandomAccessStream srcStream)
        {
            var target = await KnownFolders
                        .PicturesLibrary
                        .CreateFileAsync("new_file1.jpg", CreationCollisionOption.ReplaceExisting)
                        .AsTask();

            using (var targetStream = await target.OpenAsync(FileAccessMode.ReadWrite))
            using (var reader = new DataReader(srcStream.GetInputStreamAt(0)))
            {


                var output = targetStream.GetOutputStreamAt(0);

                await reader.LoadAsync((uint)srcStream.Size);

                while (reader.UnconsumedBufferLength > 0)
                {
                    uint dataToRead = reader.UnconsumedBufferLength > 64
                                        ? 64
                                        : reader.UnconsumedBufferLength;

                    IBuffer buffer = reader.ReadBuffer(dataToRead);

                    await output.WriteAsync(buffer);
                }

                await output.FlushAsync();
            }
            return target;
        }
    }

   
}
