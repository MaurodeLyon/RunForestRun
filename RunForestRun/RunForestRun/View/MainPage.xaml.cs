using RunForestRun.Model;
using RunForestRun.View;
using RunForestRun.ViewModel;
using System;
using System.Collections.Generic;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RunForestRun
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Controller controller;
        public MainPage()
        {
            this.InitializeComponent();
            PageName.Text = "Map";
            controller = new Controller();
            innerFrame.Navigate(typeof(Map), controller);
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = !splitView.IsPaneOpen;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Map.IsSelected)
            {
                PageName.Text = "Map";
                innerFrame.Navigate(typeof(Map), controller);
            }
            else if (Save.IsSelected)
            {
                PageName.Text = "Load";
                innerFrame.Navigate(typeof(Load), innerFrame);
            }
            else if (Compare.IsSelected)
            {
                PageName.Text = "Compare";
                innerFrame.Navigate(typeof(Compare), innerFrame);
            }
        }
    }
}
