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
        public GPS()
        {
            this.InitializeComponent();
            test();
        }

        private async void test()
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
                //geolocator.PositionChanged += GeolocatorPositionChanged;
                //GeofenceMonitor.Current.GeofenceStateChanged += GeofenceStateChanged;

                Geoposition d = await geolocator.GetGeopositionAsync();

                var pos = new Geopoint(d.Coordinate.Point.Position);
                MapIcon mapIcon1 = new MapIcon();
                
                mapIcon1.Location = pos;
                mapIcon1.NormalizedAnchorPoint = new Point(0.5, 1.0);
                mapIcon1.Title = "Lindelauf BV";
                mapIcon1.ZIndex = 0;
                
                map.MapElements.Add(mapIcon1);

                double centerLatitude = d.Coordinate.Latitude;
                double centerLongitude = d.Coordinate.Longitude;
                MapPolygon mapPolygon = new MapPolygon();
                mapPolygon.Path = new Geopath(new List<BasicGeoposition>() {
                new BasicGeoposition() {Latitude=centerLatitude+0.0005, Longitude=centerLongitude-0.001 },
                new BasicGeoposition() {Latitude=centerLatitude-0.0005, Longitude=centerLongitude-0.001 },
                new BasicGeoposition() {Latitude=centerLatitude-0.0005, Longitude=centerLongitude+0.001 },
                new BasicGeoposition() {Latitude=centerLatitude+0.0005, Longitude=centerLongitude+0.001 },

         });

                mapPolygon.ZIndex = 1;
                mapPolygon.FillColor = Colors.Red;
                mapPolygon.StrokeColor = Colors.Blue;
                mapPolygon.StrokeThickness = 3;
                mapPolygon.StrokeDashed = false;
                map.MapElements.Add(mapPolygon);

                await map.TrySetViewAsync(pos, 15);
            }
            else
            {
                //GeofenceMonitor.Current.GeofenceStateChanged -= GeofenceStateChanged;
                //geolocator.PositionChanged -= GeolocatorPositionChanged;
                Geoposition d = await geolocator.GetGeopositionAsync();

                var pos = new Geopoint(d.Coordinate.Point.Position);

                await map.TrySetViewAsync(pos,15);
                geolocator = null;
            }

            
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            const string beginLocation = "Geertruidenberg";
            const string endLocation= "Moskou, Russische Federatie";
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
            var dialog = new Windows.UI.Popups.MessageDialog(
                "Aliquam laoreet magna sit amet mauris iaculis ornare. " +
                "Morbi iaculis augue vel elementum volutpat.",
                "Lorem Ipsum");
            
            var result = await dialog.ShowAsync();

        }

        //private void GeolocatorPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        //{

        //    var pos = new Geopoint(new BasicGeoposition
        //    { Latitude = position.Latitude, Longitude = position.Longitude });

        //    DrawCarIcon(pos);


        //    await MyMap.TrySetViewAsync(pos, MyMap.ZoomLevel);
        //}
    }
}
