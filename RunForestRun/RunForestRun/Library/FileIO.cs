using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Windows.Storage;
using System.IO;
using Newtonsoft.Json;

namespace RunForestRun.Library
{
    class FileIO
    {
        public static async void Save(String filename, String contents)
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            Windows.Storage.StorageFile sampleFile = await storageFolder.CreateFileAsync(filename+ ".data", Windows.Storage.CreationCollisionOption.ReplaceExisting);

            await Windows.Storage.FileIO.WriteTextAsync(sampleFile, contents);

        }

        public static async Task<string> Load(String filename)
        {

            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            Windows.Storage.StorageFile sampleFile = await storageFolder.GetFileAsync("sample.txt");

            String contents = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);

            return contents;


        }

    }
}
