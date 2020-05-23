namespace JMangaReader.Services
{
    public interface IFileService
    {
        string SavePicture(string name, byte[] data, string location = "temp");
    }
}