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

namespace RunForestRun.ViewModel
{
    class Controller : INotifyPropertyChanged
    {
        private string _tijd;
        private string _afstand;
        private string _snelheid;
        private string _tempo;
        private DataHandler _dataHandler;

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
        public string snelheid
        {
            get { return _snelheid; }
            set
            {
                _snelheid = value;
                this.OnPropertyChanged();
            }
        }
        public string tempo
        {
            get { return _tempo; }
            set
            {
                _tempo = value;
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
            dataHandler = new DataHandler();
            _snelheid = "0";
            _afstand = "0";
            _tempo = "0";
            _tijd = "0";
            dataHandler.locator.PositionChanged += locationChanged;
            checkPosition();
        }

        private async void checkPosition()
        {
            Geoposition update = await dataHandler.locator.GetGeopositionAsync();
            currentPosition = update.Coordinate.Point;
        }

        internal void toggleRecording()
        {
            dataHandler.isWalking = !dataHandler.isWalking;
            switch (dataHandler.isWalking)
            {
                case true:
                    dataHandler.currentRoute = new Route();
                    break;
                case false:
                    if (dataHandler.currentRoute.routePoints.Count > 5)
                    {
                        dataHandler.currentRoute.eindTijd = DateTime.Now;
                        dataHandler.manifest.Add(dataHandler.currentRoute);
                        Library.FileIO.SaveManifest(dataHandler.manifest);
                    }
                    break;
            }
        }

        public Geopoint currentPosition;

        private async void locationChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Geoposition update = await dataHandler.locator.GetGeopositionAsync();
            currentPosition = update.Coordinate.Point;
            //loadInfoPage();
            if (dataHandler.isWalking)
                recording();
        }

        public async void loadInfoPage()
        {
            if (dataHandler.currentRoute.routePoints.Count >= 2)
            {
                MapRouteFinderResult routeResult = await MapRouteFinder.GetWalkingRouteFromWaypointsAsync(dataHandler.currentRoute.routePoints);
                afstand = Math.Round((routeResult.Route.LengthInMeters / 1000),2).ToString();
            }
            TimeSpan test = (DateTime.Now.TimeOfDay - dataHandler.currentRoute.beginTijd.TimeOfDay);
            int hours = test.Hours;
            int minutes = test.Minutes;
            int seconds = test.Seconds;
            String Hours, Minutes, Seconds;
            if(hours < 10)
            Hours = "0" + hours;
            else { Hours = ""+ hours; }
            if(minutes<10)
            Minutes = "0" + minutes;
            else { Minutes = "" + minutes; }
            if (seconds < 10)
            Seconds = "0" + seconds;
            else { Seconds = "" + seconds; }

            tijd = Hours+ ":" + Minutes + ":" + Seconds;
            snelheid = "♥";
            tempo = "♥";
        }

        private void recording()
        {
            dataHandler.currentRoute.routePoints.Add(currentPosition);
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
           
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
           

        }
    }
}

