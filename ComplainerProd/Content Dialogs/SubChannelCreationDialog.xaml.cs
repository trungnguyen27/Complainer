using ComplainerProd.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ComplainerProd.Content_Dialogs
{
    public sealed partial class SubChannelCreationDialog : ContentDialog
    {

        public ObservableCollection<Department> Departments { get; set; }

        public Channel Channel { get; set; }

        public TextBox SubChannelTB
        {
            get
            {
                return SubChannelNameTB;
            }
        }

        public Button DeleteBtn
        {
            get
            {
                return DeleteButton;
            }
        }

        public Button AddBtn
        {
            get
            {
                return AddButton;
            }
        }

        public Button CancelBtn
        {
            get
            {
                return CancelButton;
            }
        }

        public SubChannelCreationDialog()
        {
            this.InitializeComponent();
            AddButton.DataContext = this;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Hide();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void SubChannelName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SubChannelNameTB.Text != "")
                AddButton.IsEnabled = true;
            else AddButton.IsEnabled = false;
        }
    }
}
