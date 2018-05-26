using ComplainerProd.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ComplainerProd
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NavigationPage : Page
    {

        public static NavigationPage instance;
        #region
        public int SelectedIndex
        {
            get { return (int)GetValue(SIProp); }
            set
            {
                Debug.WriteLine(value);//writes nothing, null
                SetValueDp(SIProp, value);
                contentFrame.Navigate(MenuItem.GetMainItems()[SelectedIndex < 0 ? 0 : SelectedIndex].PageType);
            }
        }


        public static readonly DependencyProperty SIProp =
          DependencyProperty.Register("SelectedIndex", typeof(int), typeof(NavigationPage), null);

        public event PropertyChangedEventHandler PropertyChanged;

        void SetValueDp(DependencyProperty property, object value, [System.Runtime.CompilerServices.CallerMemberName] String p = null)
        {
            SetValue(property, value);
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }
        #endregion


        public NavigationPage()
        {
            this.InitializeComponent();
            instance = this;
            hamburgerMenuControl.ItemsSource = MenuItem.GetMainItems();
            hamburgerMenuControl.OptionsItemsSource = MenuItem.GetOptionsItems();
            SelectedIndex = 0;

            //if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.XamlCompositionBrushBase"))
            //{
            //    Windows.UI.Xaml.Media.AcrylicBrush myBrush = new Windows.UI.Xaml.Media.AcrylicBrush();
            //    myBrush.BackgroundSource = Windows.UI.Xaml.Media.AcrylicBackgroundSource.HostBackdrop;
            //    myBrush.TintColor = Color.FromArgb(255, 202, 24, 37);
            //    myBrush.FallbackColor = Color.FromArgb(255, 202, 24, 37);
            //    myBrush.TintOpacity = 0.6;

            //    hamburgerMenuControl.PaneBackground = myBrush;

            //    //grid.Fill = myBrush;
            //}
            //else
            //{
            //    SolidColorBrush myBrush = new SolidColorBrush(Color.FromArgb(255, 202, 24, 37));

            //    hamburgerMenuControl.PaneBackground = myBrush;
            //}
        }

        public void NavigateTo(Type pageType, object parameter = null)
        {
            contentFrame.Navigate(pageType, parameter);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void OnMenuItemClick(object sender, ItemClickEventArgs e)
        {
            var menuItem = e.ClickedItem as MenuItem;
            contentFrame.Navigate(menuItem.PageType);
            hamburgerMenuControl.IsPaneOpen = false;
            //SelectedIndex = hamburgerMenuControl.SelectedIndex;
        }

        private void hamburgerMenuControl_OptionsItemClick(object sender, ItemClickEventArgs e)
        {
            var menuItem = e.ClickedItem as MenuItem;
            contentFrame.Navigate(menuItem.PageType);
        }
    }
}
