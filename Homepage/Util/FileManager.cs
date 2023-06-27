namespace Homepage.Util;

public class FileManager
{
    public void WriteForDownload(byte[] data, string filename)
    {
        if (!Directory.Exists("./wwwroot/download"))
            Directory.CreateDirectory("./wwwroot/download");
        File.WriteAllBytes($"./wwwroot/download/{filename}", data);
    }
}