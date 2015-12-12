﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        private Geolocator geolocator;

        public Map()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
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


                await map.TrySetViewAsync(pos,50);
            }
            else
            {
                //GeofenceMonitor.Current.GeofenceStateChanged -= GeofenceStateChanged;
                //geolocator.PositionChanged -= GeolocatorPositionChanged;
                Geoposition d = await geolocator.GetGeopositionAsync();

                var pos = new Geopoint(d.Coordinate.Point.Position);


                await map.TrySetViewAsync(pos,50);
                geolocator = null;
            }
        }

        //private void GeolocatorPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        //{
            
        //    var pos = new Geopoint(new BasicGeoposition
        //    { Latitude = position.Latitude, Longitude = position.Longitude });

        //    DrawCarIcon(pos);


        //    await MyMap.TrySetViewAsync(pos, MyMap.ZoomLevel, …);
        //}

    }
}

