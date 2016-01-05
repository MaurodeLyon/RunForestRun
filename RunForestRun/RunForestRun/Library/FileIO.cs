using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Windows.Storage;
using System.IO;
using Newtonsoft.Json;
using RunForestRun.Model;

namespace RunForestRun.Library
{
    public class FileIO
    {
        //
        // save/load manifest
        //
        public static async void SaveManifest(List<string> manifest)
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var obj = JsonConvert.SerializeObject(manifest);
            Windows.Storage.StorageFile route = await storageFolder.CreateFileAsync("Manifest.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(route, obj);
        }

        public static async Task<List<string>> LoadManifest()
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            
            Windows.Storage.StorageFile manifest = await storageFolder.GetFileAsync("Manifest.txt");
            List<string> routeNameList = JsonConvert.DeserializeObject<List<string>>(await Windows.Storage.FileIO.ReadTextAsync(manifest));
            return routeNameList;
        }

        //
        // save/load route
        //
        public static async void SaveRoute(Route route, List<string> manifest)
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            manifest.Add(route.naam);
            Windows.Storage.StorageFile routeFile = await storageFolder.CreateFileAsync(route.naam + ".txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            SaveManifest(manifest);
            await Windows.Storage.FileIO.WriteTextAsync(routeFile, JsonConvert.SerializeObject(route));
        }

        public static async Task<Route> LoadRoute(string routeNaam)
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile route = await storageFolder.GetFileAsync(routeNaam + ".txt");
            return JsonConvert.DeserializeObject<Route>(await Windows.Storage.FileIO.ReadTextAsync(route));
        }

        public static async Task<List<Route>> LoadAllRoutes(List<string> manifest)
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            List<Route> routes = new List<Route>();
            foreach (string routeName in manifest)
            {
                Route r = await LoadRoute(routeName);
                routes.Add(r);
            }
            return routes;
        }
    }
}
