using RunForestRun.Library;
using RunForestRun.Model;
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
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
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
    public sealed partial class RouteInfo : Page
    {
        public RouteInfoViewModel ViewModel;
        private Controller controller;

        public RouteInfo()
        {
            this.InitializeComponent();
            ViewModel = new RouteInfoViewModel();
        }

       protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            controller = e.Parameter as Controller;
            DataContext = controller;
            if(controller.dataHandler.infoRoute !=null)
            setupMapControl(controller.dataHandler.infoRoute);
        }

        private async void setupMapControl(Route route)
        {
            List<Waypoint> waypoints = route.routePoints;
            List<BasicGeoposition> walkedPointList = new List<BasicGeoposition>();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {

                MapPolyline updatedWalkedLine = new MapPolyline
                {
                    StrokeThickness = 11,
                    StrokeColor = Colors.BurlyWood,
                    StrokeDashed = false,
                    ZIndex = 3
                };

                foreach (Waypoint wp in waypoints)
                    walkedPointList.Add(wp.GeoPosition().Position);

                updatedWalkedLine.Path = new Geopath(walkedPointList);

                map.MapElements.Add(updatedWalkedLine);


                List<Geopoint> walkedGeoPointList = new List<Geopoint>();

                foreach (Waypoint wp in waypoints)
                    walkedGeoPointList.Add(wp.GeoPosition());

                MapRouteFinderResult routeResult
                    = await MapRouteFinder.GetWalkingRouteFromWaypointsAsync(walkedGeoPointList);
                MapRoute b = routeResult.Route;
                await map.TrySetViewBoundsAsync(b.BoundingBox, null, Windows.UI.Xaml.Controls.Maps.MapAnimationKind.None);

            });

            
        }


    }
}
