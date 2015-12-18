using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RunForestRun.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Map : Page
    {
        Geoposition startpoint;
        private List<Geopoint> walkedRoute;
        private bool logging;
        public static ApplicationDataContainer LOCAL_SETTINGS = ApplicationData.Current.LocalSettings;
        public Map()
        {
            this.InitializeComponent();
            walkedRoute = new List<Geopoint>();
            LOCAL_SETTINGS.Values["logging"] = false;
            mapFrame.Navigate(typeof(GPS),walkedRoute);
            
        }
        private async void getStart()
        {
            Geolocator locator = new Geolocator();
            startpoint = await locator.GetGeopositionAsync();
        }
        private void Kaart_Click(object sender, RoutedEventArgs e)
        {
            mapFrame.Navigate(typeof(GPS),startpoint);
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            mapFrame.Navigate(typeof(Info));
            Debug.WriteLine(walkedRoute.Count);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            getStart();
            LOCAL_SETTINGS.Values["logging"] = true;
        }
    }
}
