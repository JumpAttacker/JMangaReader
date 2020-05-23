using System.IO;
using Android.OS;
using JMangaReader.Droid;
using JMangaReader.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileService))]

namespace JMangaReader.Droid
{
    public class FileService : IFileService
    {
        public string SavePicture(string name, byte[] data, string location = "temp")
        {
            var documentsPath = Path.Combine(Environment.ExternalStorageDirectory.AbsolutePath,
                Environment.DirectoryDownloads);
            documentsPath = Path.Combine(documentsPath, "Manga", location);
            Directory.CreateDirectory(documentsPath);

            var filePath = Path.Combine(documentsPath, name);

            using var fs = new FileStream(filePath, FileMode.OpenOrCreate);
            fs.Write(data, 0, data.Length);
            return filePath;
        }
    }
}