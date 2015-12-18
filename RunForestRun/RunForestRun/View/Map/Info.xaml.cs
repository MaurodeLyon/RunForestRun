using RunForestRun.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.UI.Core;
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
    public sealed partial class Info : Page
    {
        Geoposition startpoint;
        Geolocator locator;
        Controller controller;

        public Info()
        {
            this.InitializeComponent();
            locator = new Geolocator();
            locator.PositionChanged += GeolocatorPositionChanged;
            controller = new Controller();
            DataContext = controller;
        }


        private async void GeolocatorPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Geoposition currentPosition = await locator.GetGeopositionAsync();

            MapRouteFinderResult routeResult = await MapRouteFinder.GetDrivingRouteAsync(startpoint.Coordinate.Point, currentPosition.Coordinate.Point);

            controller.tijd = DateTime.Now.ToString();
            controller.afstand = (routeResult.Route.LengthInMeters).ToString();
            controller.snelheid = currentPosition.Coordinate.Speed.ToString();
            controller.tempo = 10.ToString();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            startpoint = (Geoposition)e.Parameter;
        }
    }
}
