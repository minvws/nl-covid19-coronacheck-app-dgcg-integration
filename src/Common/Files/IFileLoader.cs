namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Files
{
    public interface IFileLoader
    {
        byte[] Load(string path);
    }
}