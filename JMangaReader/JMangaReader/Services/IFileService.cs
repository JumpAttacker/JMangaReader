namespace JMangaReader.Services
{
    public interface IFileService
    {
        bool IsActive { get; set; }
        string SavePicture(string name, byte[] data, string location = "temp");
    }
}