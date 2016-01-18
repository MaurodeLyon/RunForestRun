using RunForestRun.Library;
using RunForestRun.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunForestRun.ViewModel
{
    public class RouteInfoViewModel
    {
        private Route route;
        private ObservableCollection<Waypoint> waypoints;
        public ObservableCollection<Waypoint> Waypoints { get { return waypoints; } }

        public RouteInfoViewModel()
        {
            route = DataHandler.getDataHandler().infoRoute;
            waypoints = new ObservableCollection<Waypoint>(route.routePoints);
        }
    }
}
