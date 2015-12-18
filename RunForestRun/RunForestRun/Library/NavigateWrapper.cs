using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace RunForestRun.Library
{
    class NavigateWrapper
    {
        public List<Geopoint> walkedRoute { get; }
        public bool logging { get; }

        public NavigateWrapper(List<Geopoint> list, bool logging)
        {
            walkedRoute = list;
            this.logging = logging;
        }
    }
}
