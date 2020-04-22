using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JMangaReader.Services
{
    public interface IFileService
    {
        string SavePicture(string name, byte[] data, string location = "temp");
    }


}
