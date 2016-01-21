using RunForestRun.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace RunForestRun.Library
{
    class RouteInfoWrapper
    {
        public Controller controller
        {
            get;
        }
        public Frame frame
        {
            get;
        }

        public RouteInfoWrapper(Frame frame, Controller controller)
        {
            this.frame = frame;
            this.controller = controller;
        }
    }
}
