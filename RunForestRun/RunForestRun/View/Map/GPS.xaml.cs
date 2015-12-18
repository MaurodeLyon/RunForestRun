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
        private Geolocator geolocator;
        private MapIcon mapIcon1;
        private List<Geopoint> walkedRoute;
        private MapPolyline LatestwalkedLine;

        public GPS()
        {
            this.InitializeComponent();
            walkedRoute = new List<Geopoint>();
            //debug();
            geoFencing();
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

        private async void debug()
        {
            Geolocator locator = new Geolocator();
            Geoposition position = await locator.GetGeopositionAsync();
            Geocoordinate coordinate = position.Coordinate;
            Geopoint point = coordinate.Point;
            BasicGeoposition basic = point.Position;
        }

        private async void Center_Click(object sender, RoutedEventArgs e)
        {
            if (geolocator == null)
            {
                geolocator = new Geolocator
                {
                    DesiredAccuracy = PositionAccuracy.High,
                    MovementThreshold = 1
                };

                geolocator.PositionChanged += GeolocatorPositionChanged;
                //GeofenceMonitor.Current.GeofenceStateChanged += GeofenceStateChanged;
            }
            Geoposition d = await geolocator.GetGeopositionAsync();

            var pos = new Geopoint(d.Coordinate.Point.Position);

            mapIcon1 = new MapIcon();
            mapIcon1.Location = pos;
            mapIcon1.NormalizedAnchorPoint = new Point(0.5, 1.0);
            mapIcon1.Title = "Current positions";
            mapIcon1.ZIndex = 0;

            map.MapElements.Add(mapIcon1);
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
            await map.TrySetViewAsync(pos, 17);
        }

        private async void GeolocatorPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {

            Geoposition d = await geolocator.GetGeopositionAsync();

            var pos = new Geopoint(d.Coordinate.Point.Position);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                mapIcon1.Location = pos;
            });

            await map.TrySetViewAsync(pos, 17);

            walkedRoute.Add(pos);

            if (walkedRoute.Count >= 2)
            {


                //MapRouteFinderResult routeResult
                //   = await MapRouteFinder.GetWalkingRouteFromWaypointsAsync(walkedRoute);

                //MapRoute b = routeResult.Route;


                var color = Colors.Green;
                color.A = 128;
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var walkedLine = new MapPolyline
                    {
                        StrokeThickness = 11,
                        StrokeColor = color,
                        StrokeDashed = false,
                        ZIndex = 4

                    };
                    List<BasicGeoposition> tempList = new List<BasicGeoposition>();

                    foreach (Geopoint e in walkedRoute)
                    {
                        tempList.Add(e.Position);
                    }

                    //walkedLine.Path = new Geopath(b.Path.Positions);
                    walkedLine.Path = new Geopath(tempList);
                    if (LatestwalkedLine != null)
                    {
                        map.MapElements.Remove(LatestwalkedLine);
                        LatestwalkedLine = walkedLine;
                    }
                    else
                    {
                        LatestwalkedLine = walkedLine;
                    }
                    map.MapElements.Add(LatestwalkedLine);
                });




            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            const string beginLocation = "Geertruidenberg";
            const string endLocation = "Breda";
            //"Granville, Manche, Frankrijk";

            MapLocationFinderResult result
                = await MapLocationFinder.FindLocationsAsync(beginLocation, map.Center);
            MapLocation from = result.Locations.First();

            result = await MapLocationFinder.FindLocationsAsync(endLocation, map.Center);
            MapLocation to = result.Locations.First();

            MapIcon mapIcon1 = new MapIcon();

            mapIcon1.Location = new Geopoint(to.Point.Position);
            mapIcon1.NormalizedAnchorPoint = new Point(0.5, 1.0);
            mapIcon1.Title = "Lindelauf BV (Supported by Putin)";
            mapIcon1.ZIndex = 0;
            map.MapElements.Add(mapIcon1);

            MapRouteFinderResult routeResult
                = await MapRouteFinder.GetDrivingRouteAsync(from.Point, to.Point);

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

        private async void map_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            String test = "wtfman";
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


        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Geolocator locator = new Geolocator();

            var location = await locator.GetGeopositionAsync().AsTask();



            Geofence fence
             = GeofenceMonitor.Current.Geofences.FirstOrDefault(gf => gf.Id == "currentLoc");

            if (fence == null)
            {
                GeofenceMonitor.Current.Geofences.Add(
                     new Geofence("currentLoc", new Geocircle(location.Coordinate.Point.Position, 50.0), MonitoredGeofenceStates.Entered,
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

            Geocircle geocircle = new Geocircle(to.Point.Position, 50);
            MonitoredGeofenceStates mask = MonitoredGeofenceStates.Entered | MonitoredGeofenceStates.Exited;

            GeofenceMonitor.Current.Geofences.Add(new Geofence("to", geocircle, mask, false));
        }
    }
}
