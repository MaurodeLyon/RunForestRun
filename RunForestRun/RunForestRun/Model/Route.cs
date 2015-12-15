using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace RunForestRun.Model
{
    class Route
    {
        private string _naam;

        public string naam
        {
            get { return _naam; }
            set { _naam = value; }
        }
        private DateTime _tijd;

        public DateTime tijd
        {
            get { return _tijd; }
            set { _tijd = value; }
        }

        private List<Geoposition> _routePoints;

        public List<Geoposition> routePoints
        {
            get { return _routePoints; }
            set { _routePoints = value; }
        }

    }
}
