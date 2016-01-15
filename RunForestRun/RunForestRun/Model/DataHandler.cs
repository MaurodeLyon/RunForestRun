using System;
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
        private static DataHandler dataHandler;
        private Geolocator _locator;
        private List<Route> _manifest;
        private Route _currentRoute;
        private bool _isWalking;
        private Route _routeToCompare;

        

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
        public Route routeToCompare
        {
            get { return _routeToCompare; }
            set { _routeToCompare = value; }
        }

        private DataHandler()
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

        public static DataHandler getDataHandler()
        {
            if (dataHandler == null)
            {
                dataHandler = new DataHandler();
            }
            return dataHandler;
        }
    }
}
