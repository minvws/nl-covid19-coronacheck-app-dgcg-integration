namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Files
{
    public interface IFileLoader
    {
        byte[] Load(string path);
    }
}