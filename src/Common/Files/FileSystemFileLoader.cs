namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Files
{
    public class FileSystemFileLoader : IFileLoader
    {
        public byte[] Load(string path)
        {
            return System.IO.File.ReadAllBytes(path);
        }
    }
}