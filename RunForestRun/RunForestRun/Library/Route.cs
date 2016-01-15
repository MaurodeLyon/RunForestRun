using RunForestRun.Library;
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
        private DateTime _beginTijd;
        private DateTime _eindTijd;
        private List<Waypoint> _routePoints;

        public string naam
        {
            get { return _naam; }
            set { _naam = value; }
        }
        public DateTime eindTijd
        {
            get { return _eindTijd; }
            set { _eindTijd = value; }
        }
        public DateTime beginTijd
        {
            get { return _beginTijd; }
            set { _beginTijd = value; }
        }
        public List<Waypoint> routePoints
        {
            get { return _routePoints; }
            set { _routePoints = value; }
        }

        public Route()
        {
            _beginTijd = DateTime.Now;
            _routePoints = new List<Waypoint>();
        }

        public Route(string naam,DateTime begintijd,DateTime eindtijd,List<Waypoint> routePoints)
        {
            _naam = naam;
            _beginTijd = begintijd;
            _eindTijd = eindTijd;
            _routePoints = routePoints;
        }
    }
}
