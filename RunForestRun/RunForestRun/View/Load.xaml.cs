using RunForestRun.Model;
using RunForestRun.ViewModel;
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
    public sealed partial class Load : Page
    {
        public List<Route> RouteList;
        private Controller controller;
        public ObservableCollection<string> selectableList = new ObservableCollection<string>();

        public Load()
        {
            this.InitializeComponent();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {

        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            controller = (Controller)e.Parameter;
            controller.dataHandler.manifest = await Library.FileIO.LoadManifest();
            RouteList = controller.dataHandler.manifest;
            foreach (Route item in RouteList)
                selectableList.Add(item.beginTijd.ToString());
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var a = e.AddedItems;
        }
    }
}
