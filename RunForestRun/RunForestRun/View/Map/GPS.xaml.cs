using RunForestRun.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
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
    public sealed partial class GPS : Page
    {
        private MapIcon currentPosIcon;
        private Controller controller;

        public GPS()
        {
            this.InitializeComponent();
            currentPosIcon = new MapIcon();
            currentPosIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
            currentPosIcon.Title = "Current position";
            currentPosIcon.ZIndex = 4;
            map.MapElements.Add(currentPosIcon);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            controller = (Controller)e.Parameter;
            controller.dataHandler.locator.PositionChanged += GeolocatorPositionChanged;
        }


        private async void Center_Click(object sender, RoutedEventArgs e)
        {
            currentPosIcon.Location = controller.currentPosition.Coordinate.Point;
            await map.TrySetViewAsync(controller.currentPosition.Coordinate.Point, 17);
        }

        private async void GeolocatorPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            currentPosIcon.Location = controller.currentPosition.Coordinate.Point;
            if (controller.dataHandler.isWalking)
            {

                await map.TrySetViewAsync(controller.currentPosition.Coordinate.Point, 17);

                if (controller.dataHandler.currentRoute.routePoints.Count >= 2)
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        MapPolyline walkedLine = new MapPolyline
                        {
                            StrokeThickness = 11,
                            StrokeColor = Colors.Gray,
                            StrokeDashed = false,
                            ZIndex = 3
                        };

                        List<BasicGeoposition> basicPositionList = new List<BasicGeoposition>();

                        foreach (Geoposition item in controller.dataHandler.currentRoute.routePoints)
                        {
                            basicPositionList.Add(item.Coordinate.Point.Position);
                        }

                        walkedLine.Path = new Geopath(basicPositionList);

                        map.MapElements.Add(walkedLine);
                    });
                }
            }
        }

        private async void TestRoute_Click(object sender, RoutedEventArgs e)
        {
            const string beginLocation = "Geertruidenberg";
            const string endLocation = "Breda";
            //"Granville, Manche, Frankrijk";

            MapLocationFinderResult result = await MapLocationFinder.FindLocationsAsync(beginLocation, map.Center);
            MapLocation from = result.Locations.First();

            result = await MapLocationFinder.FindLocationsAsync(endLocation, map.Center);
            MapLocation to = result.Locations.First();

            MapIcon mapIcon1 = new MapIcon();

            mapIcon1.Location = new Geopoint(to.Point.Position);
            mapIcon1.NormalizedAnchorPoint = new Point(0.5, 1.0);
            mapIcon1.Title = "Lindelauf BV (Supported by Putin)";
            mapIcon1.ZIndex = 4;
            map.MapElements.Add(mapIcon1);

            MapRouteFinderResult routeResult = await MapRouteFinder.GetDrivingRouteAsync(from.Point, to.Point);

            MapRoute b = routeResult.Route;

            var color = Colors.Green;
            color.A = 128;

            var line = new MapPolyline
            {
                StrokeThickness = 11,
                StrokeColor = color,
                StrokeDashed = false,
                ZIndex = 2
            };
            line.Path = new Geopath(b.Path.Positions);
            map.MapElements.Add(line);
        }

        /*private async void map_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            string test = "wtfman";
            if (args.MapElements.First() is MapIcon)
            {
                MapIcon two = (MapIcon)args.MapElements.First();
                test = two.Title;
            }
            var dialog = new Windows.UI.Popups.MessageDialog(
                "Aliquam laoreet magna sit amet mauris iaculis ornare. " +
                "Morbi iaculis augue vel elementum volutpat.",
                "Lorem Ipsum" + test);
            var result = await dialog.ShowAsync();
        }*/

        private async void TestGeoFence_Click(object sender, RoutedEventArgs e)
        {
            GeofenceMonitor.Current.Geofences.Clear();
            Geolocator locator = new Geolocator();
            var location = await locator.GetGeopositionAsync().AsTask();
            Geofence fence = GeofenceMonitor.Current.Geofences.FirstOrDefault(gf => gf.Id == "currentLoc");

            if (fence == null)
            {
                GeofenceMonitor.Current.Geofences.Add(
                     new Geofence("currentLoc", new Geocircle(location.Coordinate.Point.Position, 10.0), MonitoredGeofenceStates.Entered,
                                    false, TimeSpan.FromSeconds(10))
                        );
            }
            GeofenceMonitor.Current.GeofenceStateChanged += GeofenceStateChanged;


            const string beginLocation = "Lovensdijkstraat, Breda";
            const string endLocation = "Hogeschoollaan,Breda";
            //"Granville, Manche, Frankrijk";

            MapLocationFinderResult result
                = await MapLocationFinder.FindLocationsAsync(beginLocation, map.Center);
            MapLocation from = result.Locations.First();

            result = await MapLocationFinder.FindLocationsAsync(endLocation, map.Center);
            MapLocation to = result.Locations.First();

            MapRouteFinderResult routeResult
                = await MapRouteFinder.GetDrivingRouteAsync(from.Point, to.Point);



            MapRoute b = routeResult.Route;


            var color = Colors.Turquoise;
            //color.A = 128;

            var line = new MapPolyline
            {
                StrokeThickness = 11,
                StrokeColor = color,
                StrokeDashed = false,
                ZIndex = 2
            };

            line.Path = new Geopath(b.Path.Positions);

            map.MapElements.Add(line);

            Geocircle geocircle = new Geocircle(to.Point.Position, 10);
            MonitoredGeofenceStates mask = MonitoredGeofenceStates.Entered | MonitoredGeofenceStates.Exited;

            GeofenceMonitor.Current.Geofences.Add(new Geofence("to", geocircle, mask, false, new TimeSpan(0)));
        }

        private async void GeofenceStateChanged(GeofenceMonitor sender, object args)
        {
            var reports = sender.ReadReports();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                foreach (GeofenceStateChangeReport report in reports)
                {
                    GeofenceState state = report.NewState;
                    Geofence geofence = report.Geofence;

                    if (state == GeofenceState.Removed)
                    {

                    }

                    else if (state == GeofenceState.Entered)
                    {
                        var dialog = new Windows.UI.Popups.MessageDialog(geofence.Id + "Entered");
                        var result = await dialog.ShowAsync();



                    }

                    else if (state == GeofenceState.Exited)
                    {

                    }
                }
            });

        }
    }
}
//       double centerLatitude = d.Coordinate.Latitude;
//       double centerLongitude = d.Coordinate.Longitude;
//       MapPolygon mapPolygon = new MapPolygon();
//       mapPolygon.Path = new Geopath(new List<BasicGeoposition>() {
//       new BasicGeoposition() {Latitude=centerLatitude+0.0005, Longitude=centerLongitude-0.001 },
//       new BasicGeoposition() {Latitude=centerLatitude-0.0005, Longitude=centerLongitude-0.001 },
//       new BasicGeoposition() {Latitude=centerLatitude-0.0005, Longitude=centerLongitude+0.001 },
//       new BasicGeoposition() {Latitude=centerLatitude+0.0005, Longitude=centerLongitude+0.001 },
//});
//       mapPolygon.ZIndex = 1;
//       mapPolygon.FillColor = Colors.Red;
//       mapPolygon.StrokeColor = Colors.Blue;
//       mapPolygon.StrokeThickness = 3;
//       mapPolygon.StrokeDashed = false;
//       map.MapElements.Add(mapPolygon);