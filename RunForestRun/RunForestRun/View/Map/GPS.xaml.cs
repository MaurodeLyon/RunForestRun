using RunForestRun.Library;
using RunForestRun.Model;
using RunForestRun.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Notifications;
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
        private bool loggedRouteSetup;

        public GPS()
        {
            this.InitializeComponent();
            currentPosIcon = new MapIcon();
            currentPosIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
            currentPosIcon.Title = "Current position";
            currentPosIcon.ZIndex = 4;
            map.MapElements.Add(currentPosIcon);
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            controller = (Controller)e.Parameter;
            controller.dataHandler.locator.PositionChanged += GeolocatorPositionChanged;

            if(!controller.GeoFencingSetup)
            {
                setupGeofencing();
                controller.GeoFencingSetup = true;
            }

            if(controller.dataHandler.routeToCompare!=null && !loggedRouteSetup)
            {
                drawLoggedRoute(controller.dataHandler.routeToCompare);
                setupGeofences(controller.dataHandler.routeToCompare);
                loggedRouteSetup = true;
            }
        }

        private void setupGeofences(Route route)
        {
            List<Waypoint> waypoints = route.routePoints;
            List<BasicGeoposition> walkedPointList = new List<BasicGeoposition>();

            foreach(Waypoint wp in waypoints)
                walkedPointList.Add(wp.GeoPosition().Position);

            Geopoint middle = waypoints[waypoints.Count / 2].GeoPosition();
            BasicGeoposition first = walkedPointList.First();
            BasicGeoposition last = walkedPointList.Last();

            setupGeofence(first, "Start");
            setupGeofence(last, "End");

            addMapIcon(middle);

        }

        public void pushNot(string title, string desc)
        {
            ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(title));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(desc));

            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            XmlElement test = toastXml.CreateElement("test");

            test.SetAttribute("src", "ms-winsoundevent:Notification.IM");

            toastNode.AppendChild(test);

            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

    

    private async void addMapIcon(Geopoint pos)
        {
            MapIcon mapIcon = new MapIcon();
            mapIcon.Location = pos;
            mapIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
            mapIcon.Title = "Logged Route";
            mapIcon.ZIndex = 4;
            //mapIcon.Image = await StorageFile.GetFileFromApplicationUriAsync(uri1);

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                map.MapElements.Add(mapIcon);
            });
        }
        private async void Center_Click(object sender, RoutedEventArgs e)
        {
            if (controller.currentPosition != null)
            {
                currentPosIcon.Location = controller.currentPosition.Point;
                await map.TrySetViewAsync(controller.currentPosition.Point, 17);

            }
        }

        private async void drawLoggedRoute(Route route)
        {
            List<Waypoint> waypoints = route.routePoints;
            List<BasicGeoposition> walkedPointList = new List<BasicGeoposition>();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
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

            });

        }

        private async void GeolocatorPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            if (controller.currentPosition != null)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    currentPosIcon.Location = controller.currentPosition.Point;
                });
                if (controller.dataHandler.isWalking)
                {

                    await map.TrySetViewAsync(controller.currentPosition.Point, 17);

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
                            List<Waypoint> secondList = new List<Waypoint>();
                            secondList.AddRange(controller.dataHandler.currentRoute.routePoints);
                            List<BasicGeoposition> basicPositionList = new List<BasicGeoposition>();
                            foreach (Waypoint item in secondList)
                            {
                                basicPositionList.Add(new BasicGeoposition() { Latitude = item.latitude, Longitude = item.longitude });
                            }

                            walkedLine.Path = new Geopath(basicPositionList);

                            map.MapElements.Add(walkedLine);
                        });
                    }
                }
            }

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
               controller.loadInfoPage(); 
            });
            
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

        private void setupGeofence(BasicGeoposition pos,String name)
        {
            Geofence fence = new Geofence(
                name,
                new Geocircle(pos, 18),
                MonitoredGeofenceStates.Entered | MonitoredGeofenceStates.Exited,
                false,
                new TimeSpan(0));
            if (!GeofenceMonitor.Current.Geofences.Contains(fence))
                GeofenceMonitor.Current.Geofences.Add(fence);
        }

        public async void setupGeofencing()
        {
            GeofenceMonitor.Current.Geofences.Clear();
            if (await Geolocator.RequestAccessAsync() == GeolocationAccessStatus.Allowed)
            {
                Geoposition location = await controller.dataHandler.locator.GetGeopositionAsync().AsTask();
                Geofence fence = GeofenceMonitor.Current.Geofences.FirstOrDefault(gf => gf.Id == "currentLoc");

                if (fence == null)
                    GeofenceMonitor.Current.Geofences.Add(
                         new Geofence("currentLoc",
                         new Geocircle(location.Coordinate.Point.Position, 10.0),
                         MonitoredGeofenceStates.Entered,
                         false,
                         TimeSpan.FromSeconds(10)));

                GeofenceMonitor.Current.GeofenceStateChanged += GeofenceStateChanged;
            }
        }

        private void GeofenceStateChanged(GeofenceMonitor sender, object args)
        {
            var reports = sender.ReadReports();
            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            //{
                foreach (GeofenceStateChangeReport report in reports)
                {
                    GeofenceState state = report.NewState;
                    Geofence geofence = report.Geofence;

                    if (state == GeofenceState.Removed)
                    {

                    }

                    else if (state == GeofenceState.Entered)
                    {
                        if (geofence.Id != "currentLoc")
                        {
                            //var dialog = new Windows.UI.Popups.MessageDialog(geofence.Id + "Entered");
                            //var result = await dialog.ShowAsync();

                            switch(geofence.Id)
                            {
                                case "Start":
                                    //var dialog = new Windows.UI.Popups.MessageDialog("You have reached the starting point of an earlier walked route on: " +
                                        //controller.dataHandler.routeToCompare.beginTijd.ToString());
                                   // var result = await dialog.ShowAsync();
                                    pushNot("Start of logged route reached!", "You have reached the starting point of an earlier walked route on: " +
                                        controller.dataHandler.routeToCompare.beginTijd.ToString());
                                    break;
                                case "End":
                                    pushNot("End of logged route reached!", "You have reached the end of an earlier walked route on: " +
                                        controller.dataHandler.routeToCompare.beginTijd.ToString());
                                    break;

                            }
                        }


                    }

                    else if (state == GeofenceState.Exited)
                    {

                    }
                }
            //});

        }
    }
}