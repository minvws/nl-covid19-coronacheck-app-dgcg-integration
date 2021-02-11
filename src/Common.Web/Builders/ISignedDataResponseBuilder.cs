using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Models;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Builders
{
    public interface ISignedDataResponseBuilder
    {
        SignedDataResponse<T> Build<T>(T responseDto);
    }
}