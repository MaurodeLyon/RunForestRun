using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace RunForestRun.Model
{
    public class Route
    {
        private string _naam;

        public string naam
        {
            get { return _naam; }
            set { _naam = value; }
        }
        private DateTime _beginTijd;

        public DateTime beginTijd
        {
            get { return _beginTijd; }
            set { _beginTijd = value; }
        }

        private DateTime _eindTijd;

        public DateTime eindTijd
        {
            get { return _eindTijd; }
            set { _eindTijd = value; }
        }


        private List<Geocoordinate> _routePoints;

        public List<Geocoordinate> routePoints
        {
            get { return _routePoints; }
            set { _routePoints = value; }
        }

        public Route(string naam)
        {
            _naam = naam;
            _routePoints = new List<Geocoordinate>();
        }
    }
}
