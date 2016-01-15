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
        public static async void SaveManifest(List<Route> manifest)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            string serializedManifest = JsonConvert.SerializeObject(manifest);
            StorageFile manifestFile = await storageFolder.CreateFileAsync("Manifest.txt", CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(manifestFile, serializedManifest);
        }

        public static async Task<List<Route>> LoadManifest()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile manifest;
            try
            {
                manifest = await storageFolder.GetFileAsync("Manifest.txt");
            }
            catch (FileNotFoundException ex)
            {
                System.Diagnostics.Debug.WriteLine("error loading manifest: " + ex);
                return null;
            }
            List<Route> routes = JsonConvert.DeserializeObject<List<Route>>(await Windows.Storage.FileIO.ReadTextAsync(manifest));
            if (routes == null)
                return new List<Route>();
            else
                return routes;
        }
    }
}
