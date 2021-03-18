namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Config
{
    public interface IRedisSessionStoreConfig
    {
        string InstanceName { get; }
        string Configuration { get; }
    }
}
