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

namespace RunForestRun.ViewModel
{
    class Controller : INotifyPropertyChanged
    {
        private string _tijd;

        public string tijd
        {
            get { return _tijd; }
            set
            {
                _tijd = value;
                this.OnPropertyChanged();
            }
        }

        private string _afstand;

        public string afstand
        {
            get { return _afstand; }
            set
            {
                _afstand = value;
                this.OnPropertyChanged();
            }
        }

        private string _snelheid;

        public string snelheid
        {
            get { return _snelheid; }
            set
            {
                _snelheid = value;
                this.OnPropertyChanged();
            }
        }

        private string _tempo;

        public string tempo
        {
            get { return _tempo; }
            set
            {
                _tempo = value;
                this.OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        internal void startRecording()
        {
            dataHandler.isWalking = true;
        }

        private DataHandler _dataHandler;

        public DataHandler dataHandler
        {
            get { return _dataHandler; }
            set { _dataHandler = value; }
        }

        public Controller()
        {
            dataHandler = new DataHandler();
            dataHandler.locator.PositionChanged += GeolocatorPositionChanged;
            _snelheid = "0";
            _afstand = "0";
            _tempo = "0";
            _tijd = "0";
        }

        private async void GeolocatorPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Geoposition currentPosition = await dataHandler.locator.GetGeopositionAsync();
            List<Geopoint> tempList = new List<Geopoint>();

            foreach (Geoposition item in dataHandler.currentRoute)
            {
                tempList.Add(item.Coordinate.Point);
            }
            if (tempList.Count >= 2)
            {
                MapRouteFinderResult routeResult = await MapRouteFinder.GetWalkingRouteFromWaypointsAsync(tempList);
                afstand = (routeResult.Route.LengthInMeters).ToString();
            }

            tijd = DateTime.Now.ToString();
            snelheid = currentPosition.Coordinate.Speed.ToString();
            tempo = 10.ToString();
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
