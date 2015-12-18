using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace RunForestRun.Library
{
    public class geolocator
    {
        // Thread-safe oplossing om slechts één instantie aan te maken.
        private static Geolocator _locator = new Geolocator();

        // Private constructor om te voorkomen dat anderen een instantie kunnen aanmaken.
        private geolocator() { }

        // Via een static read-only property kan de instantie benaderd worden.
        public static Geolocator Geolocator
        {
            get
            {
                return _locator;
            }
        }
    }
}