﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;

namespace RunForestRun.Model
{
    class DataHandler
    {
        private Geolocator _locator;
        private List<Route> _manifest;
        private Route _currentRoute;
        private bool _isWalking;

        public Geolocator locator
        {
            get { return _locator; }
            set { _locator = value; }
        }
        public List<Route> manifest
        {
            get { return _manifest; }
            set { _manifest = value; }
        }
        public Route currentRoute
        {
            get { return _currentRoute; }
            set { _currentRoute = value; }
        }
        public bool isWalking
        {
            get { return _isWalking; }
            set { _isWalking = value; }
        }

        public DataHandler()
        {
            _locator = new Geolocator();
            _currentRoute = new Route();
            _isWalking = false;
            loadManifest();
        }

        private async void loadManifest()
        {
            _manifest = await Library.FileIO.LoadManifest();
        }
    }
}
