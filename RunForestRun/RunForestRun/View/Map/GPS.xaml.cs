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
using Windows.UI.Popups;
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
        private bool GeoFencingSetup;

        public GPS()
        {
            this.InitializeComponent();
            currentPosIcon = new MapIcon();
            currentPosIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
            currentPosIcon.Title = "Current position";
            currentPosIcon.ZIndex = 4;
            map.MapElements.Add(currentPosIcon);
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            GeoFencingSetup = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            controller = (Controller)e.Parameter;
            controller.dataHandler.locator.PositionChanged += GeolocatorPositionChanged;

            if (!GeoFencingSetup)
            {
                setupGeofencing();
                GeoFencingSetup = true;
            }

            if (controller.dataHandler.routeToCompare != null && !loggedRouteSetup)
            {
                drawLoggedRoute(controller.dataHandler.routeToCompare);
                setupStartAndFinish(controller.dataHandler.routeToCompare);
                loggedRouteSetup = true;
            }
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
                if (controller.dataHandler.isRecording)
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

        private async void setupGeofencing()
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
            await map.TrySetViewAsync(controller.currentPosition.Point, 17);
        }

        private void setupStartAndFinish(Route route)
        {
            List<Waypoint> waypoints = new List<Waypoint>();
            waypoints.AddRange(route.routePoints);
            List<BasicGeoposition> walkedPointList = new List<BasicGeoposition>();
            foreach (Waypoint wp in waypoints)
                walkedPointList.Add(wp.GeoPosition().Position);

            Geopoint middle = waypoints[waypoints.Count / 2].GeoPosition();
            BasicGeoposition first = walkedPointList.First();
            BasicGeoposition last = walkedPointList.Last();

            setupGeofence(first, "Start");
            setupGeofence(last, "End");

            addMapIcon(middle);
        }

        private void setupGeofence(BasicGeoposition pos, string name)
        {
            Geofence fence = new Geofence(
                name,
                new Geocircle(pos, 1),
                MonitoredGeofenceStates.Entered | MonitoredGeofenceStates.Exited,
                false,
                new TimeSpan(0));
            if (!GeofenceMonitor.Current.Geofences.Contains(fence))
                GeofenceMonitor.Current.Geofences.Add(fence);
        }

        private async void GeofenceStateChanged(GeofenceMonitor sender, object args)
        {
            var reports = sender.ReadReports();
            foreach (GeofenceStateChangeReport report in reports)
            {
                switch (report.NewState)
                {
                    case GeofenceState.None:
                        break;
                    case GeofenceState.Entered:
                        if (report.Geofence.Id != "currentLoc")
                            switch (report.Geofence.Id)
                            {
                                case "Start":
                                    pushnotification.pushNot("Start of logged route reached!", "You have reached the starting point of an earlier walked route on: " +
                                        controller.dataHandler.routeToCompare.beginTijd.ToString());
                                    controller.toggleRecording();
                                    break;
                                case "End":
                                    controller.toggleRecording();
                                    var nieuweTijd = DataHandler.getDataHandler().currentRoute.eindTijd - DataHandler.getDataHandler().currentRoute.beginTijd;
                                    var oudeTijd = DataHandler.getDataHandler().routeToCompare.eindTijd - DataHandler.getDataHandler().routeToCompare.beginTijd;
                                    if (nieuweTijd < oudeTijd)
                                    {
                                        pushnotification.pushNot("End of logged route reached!", "You were faster!!!");
                                        var dialog = new MessageDialog("Old time: " + oudeTijd.ToString() + "\nNew time: " + nieuweTijd.ToString(), "End of logged route reached!");
                                        await dialog.ShowAsync();
                                    }
                                    else
                                    {
                                        pushnotification.pushNot("End of logged route reached!", "You were slower.");
                                        var dialog = new MessageDialog("Old time: " + oudeTijd.ToString() + "\nNew time: " + nieuweTijd.ToString(), "End of logged route reached!");
                                        await dialog.ShowAsync();
                                    }
                                    break;
                            }
                        break;
                    case GeofenceState.Exited:
                        break;
                    case GeofenceState.Removed:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}