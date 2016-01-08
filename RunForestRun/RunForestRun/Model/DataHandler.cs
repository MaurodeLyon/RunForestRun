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
        private List<string> _manifest;
        private List<Route> _routes;
        private Geolocator _locator;
        private List<Geoposition> _currentRoute;
        private bool _isWalking;

        public List<string> manifest
        {
            get { return _manifest; }
            set { _manifest = value; }
        }
        public List<Route> routes
        {
            get { return _routes; }
            set { _routes = value; }
        }
        public Geolocator locator
        {
            get { return _locator; }
            set { _locator = value; }
        }
        public List<Geoposition> currentRoute
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
            _currentRoute = new List<Geoposition>();
            _routes = new List<Route>();
            _isWalking = false;
            load();
        }

        private async void load()
        {
            _manifest = await Library.FileIO.LoadManifest();
            _routes = await Library.FileIO.LoadAllRoutes(_manifest);
        }
    }
}
