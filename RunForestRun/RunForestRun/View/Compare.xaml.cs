using RunForestRun.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RunForestRun.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Compare : Page
    {
        public List<Route> RouteList;
        public ObservableCollection<string> selectableList = new ObservableCollection<string>();
        private Frame innerFrame;

        public Compare()
        {
            this.InitializeComponent();
        }


        private void Info_Click(object sender, RoutedEventArgs e)
        {
            if (list.SelectedItem != null)
            {
                string selectedList = list.SelectedItem as string;
                foreach (Route item in RouteList)
                    if (item.beginTijd.ToString() == selectedList.ToString())
                        DataHandler.getDataHandler().routeToCompare = item;
                innerFrame.Navigate(typeof(Map));
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            innerFrame = e.Parameter as Frame;
            DataHandler.getDataHandler().manifest = await Library.FileIO.LoadManifest();
            RouteList = DataHandler.getDataHandler().manifest;
            foreach (Route item in RouteList)
                selectableList.Add(item.beginTijd.ToString());
        }
    }
}
