using RunForestRun.Model;
using RunForestRun.ViewModel;
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
        Controller controller;
        public Map()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            Race.IsEnabled = false;
        }

        private void Kaart_Click(object sender, RoutedEventArgs e)
        {
            mapFrame.Navigate(typeof(GPS), controller);
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            mapFrame.Navigate(typeof(Info), controller);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            controller.toggleRecording();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                controller = e.Parameter as Controller;
            }
            else {
                DataHandler.getDataHandler().isWalking = false;
            }
            if (controller.dataHandler.routeToCompare != null)
            {
                Race.IsEnabled = true;
            }
            mapFrame.Navigate(typeof(GPS), controller);
        }

        private void Info_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Race_Click(object sender, RoutedEventArgs e)
        {
            mapFrame.Navigate(typeof(Race), controller);
        }
    }
}
