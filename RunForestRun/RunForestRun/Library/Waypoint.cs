using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunForestRun.Library
{
    public class Waypoint
    {
        private double _latitude;
        private double _longitude;
        private double? _speed;
        private DateTimeOffset _timestamp;

        public double latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }
        public double longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }
        public double? speed
        {
            get { return _speed; }
            set { _speed = value; }
        }
        public DateTimeOffset timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

        public Waypoint(double latitude, double longitude, double? speed, DateTimeOffset timestamp)
        {
            _latitude = latitude;
            _longitude = longitude;
            _speed = speed;
            _timestamp = timestamp;
        }
    }
}
