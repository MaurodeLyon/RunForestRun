using RunForestRun.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.UI.Core;
using System.ServiceModel;
using Windows.UI.Xaml;
using RunForestRun.Library;

namespace RunForestRun.ViewModel
{
    class Controller : INotifyPropertyChanged
    {
        private string _tijd;
        private string _afstand;
        private string _snelheid;
        private DataHandler _dataHandler;

        public string logTijd
        {
            get
            {
                TimeSpan t = _dataHandler.routeToCompare.eindTijd - _dataHandler.routeToCompare.beginTijd;
                return t.ToString().Split('.')[0];
            }
        }
        public string logAfstand
        {
            get
            {
                TimeSpan t = _dataHandler.routeToCompare.eindTijd - _dataHandler.routeToCompare.beginTijd;

                double afstand = t.TotalSeconds * double.Parse(logSnelheid);
                afstand = afstand / 1000;
                afstand = Math.Round(afstand, 2);
                return afstand.ToString();
            }
        }
        public string logSnelheid
        {
            get
            {
                double total = 0;
                foreach (Waypoint wp in _dataHandler.routeToCompare.routePoints)
                    total += (double)wp.speed;
                total = total / _dataHandler.routeToCompare.routePoints.Count;
                return Math.Round(total, 2).ToString();

            }
        }
        //the loggedTijd, loggedAfstand and loggedSnelheid attributes are used by the RouteInfo page. the Log attributes are used by the Compare page
        public string loggedTijd
        {
            get
            {
                TimeSpan t = _dataHandler.infoRoute.eindTijd - _dataHandler.infoRoute.beginTijd;
                return t.ToString().Split('.')[0];
            }
        }
        public string loggedAfstand
        {
            get
            {
                TimeSpan t = _dataHandler.infoRoute.eindTijd - _dataHandler.infoRoute.beginTijd;

                double afstand = t.TotalSeconds * double.Parse(loggedSnelheid);
                afstand = afstand / 1000;
                afstand = Math.Round(afstand, 2);
                return afstand.ToString();
            }
        }
        public string loggedSnelheid
        {
            get
            {
                double total = 0;
                foreach (Waypoint wp in _dataHandler.infoRoute.routePoints)
                    total += (double)wp.speed;
                total = total / _dataHandler.infoRoute.routePoints.Count;
                return Math.Round(total, 2).ToString();

            }
        }

        public string logBeginTijd
        {
            get
            {
                return _dataHandler.infoRoute.beginTijd.ToString();
            }
        }

        public string logMaxSnelheid
        {
            get
            {
                double max = 0;
                foreach (Waypoint wp in _dataHandler.infoRoute.routePoints)
                    if (max < wp.speed) max = (double)wp.speed;
                return Math.Round(max, 2).ToString();
            }
        }

        public string logMinSnelheid
        {
            get
            {
                double min = 0;
                foreach (Waypoint wp in _dataHandler.infoRoute.routePoints)
                    if (min > wp.speed) min = (double)wp.speed;
                return Math.Round(min, 2).ToString();
            }
        }
        public string tijd
        {
            get { return _tijd; }
            set
            {
                _tijd = value;
                this.OnPropertyChanged();
            }
        }
        public string afstand
        {
            get { return _afstand; }
            set
            {
                _afstand = value;
                this.OnPropertyChanged();
            }
        }


        public Geocoordinate currentPosition;
        public string snelheid
        {
            get { return _snelheid; }
            set
            {
                _snelheid = value;
                this.OnPropertyChanged();
            }
        }
        public DataHandler dataHandler
        {
            get { return _dataHandler; }
            set { _dataHandler = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public Controller()
        {
            dataHandler = DataHandler.getDataHandler();
            _snelheid = "0";
            _afstand = "0";
            _tijd = "0";
            dataHandler.locator.PositionChanged += locationChanged;
            checkPosition();
        }


        internal void toggleRecording()
        {
            dataHandler.isRecording = !dataHandler.isRecording;
            switch (dataHandler.isRecording)
            {
                case true:
                    dataHandler.currentRoute = new Route();
                    break;
                case false:
                    if (dataHandler.currentRoute.routePoints.Count > 5)
                    {
                        dataHandler.currentRoute.eindTijd = DateTime.Now;
                        dataHandler.manifest.Add(dataHandler.currentRoute);
                        FileIO.SaveManifest(dataHandler.manifest);
                        _snelheid = "0";
                        _afstand = "0";
                        _tijd = "0";
                    }
                    break;
            }
        }

        private async void checkPosition()
        {
            var a = await Geolocator.RequestAccessAsync();
            if (a == GeolocationAccessStatus.Allowed)
            {
                Geoposition update = await dataHandler.locator.GetGeopositionAsync();
                currentPosition = update.Coordinate;
            }
        }

        private async void locationChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            var a = await Geolocator.RequestAccessAsync();
            if (a == GeolocationAccessStatus.Allowed)
            {
                Geoposition update = await dataHandler.locator.GetGeopositionAsync();
                currentPosition = update.Coordinate;
                //loadInfoPage();
                if (dataHandler.isRecording)
                    recording();
            }
        }

        public async void loadInfoPage()
        {
            if (currentPosition != null)
            {


                if (dataHandler.isRecording)
                {
                    //Distance
                    if (dataHandler.currentRoute.routePoints.Count >= 2)
                    {
                        MapRouteFinderResult routeResult = await MapRouteFinder.GetWalkingRouteFromWaypointsAsync(createGeoPointList());
                        afstand = Math.Round((routeResult.Route.LengthInMeters / 1000), 2).ToString();
                    }
                    //Time
                    TimeSpan timespan = (DateTime.Now.TimeOfDay - dataHandler.currentRoute.beginTijd.TimeOfDay);
                    int hours = timespan.Hours;
                    int minutes = timespan.Minutes;
                    int seconds = timespan.Seconds;
                    string Hours, Minutes, Seconds;
                    if (hours < 10) Hours = "0" + hours;
                    else Hours = "" + hours;
                    if (minutes < 10) Minutes = "0" + minutes;
                    else Minutes = "" + minutes;
                    if (seconds < 10) Seconds = "0" + seconds;
                    else Seconds = "" + seconds;
                    tijd = Hours + ":" + Minutes + ":" + Seconds;
                }
                //speed
                double speed = 0;
                if (currentPosition.Speed.HasValue)
                {
                    double.TryParse(currentPosition.Speed.Value.ToString(), out speed);
                    speed = Math.Round(speed, 0);
                }
                snelheid = speed.ToString();
            }
        }

        private void recording()
        {
            dataHandler.currentRoute.routePoints.Add(new Waypoint(currentPosition.Point.Position.Latitude, currentPosition.Point.Position.Longitude, currentPosition.Speed, currentPosition.Timestamp));
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.

            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));


        }

        private List<Geopoint> createGeoPointList()
        {
            List<Waypoint> secondList = new List<Waypoint>();
            secondList.AddRange(dataHandler.currentRoute.routePoints);
            List<Geopoint> toCreate = new List<Geopoint>();
            foreach (Waypoint w in secondList)
            {
                toCreate.Add(new Geopoint(new BasicGeoposition() { Longitude = w.longitude, Latitude = w.latitude }));
            }
            return toCreate;
        }

    }
}

