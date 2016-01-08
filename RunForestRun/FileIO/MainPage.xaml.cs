using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FileIO
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            testMethod();
        }

        public async void testMethod()
        {
            StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await storageFolder.CreateFileAsync("sample.txt", CreationCollisionOption.ReplaceExisting);
            sampleFile = await storageFolder.GetFileAsync("sample.txt");

            await Windows.Storage.FileIO.WriteTextAsync(sampleFile, JsonConvert.SerializeObject(new Route(0, "Hello ♥")));
            sampleFile = await storageFolder.GetFileAsync("sample.txt");
            Route text = JsonConvert.DeserializeObject<Route>(await Windows.Storage.FileIO.ReadTextAsync(sampleFile));
        }

        public class Route
        {
            public int id;
            public string description;

            public Route(int id, string description)
            {
                this.id = id;
                this.description = description;
            }
        }
    }
}