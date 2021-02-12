using System;
using System.Text;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Signing;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Models;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Builders
{
    public class SignedDataResponseBuilder : ISignedDataResponseBuilder
    {
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IContentSigner _signer;

        public SignedDataResponseBuilder(IJsonSerializer jsonSerializer, IContentSigner signer)
        {
            _signer = signer ?? throw new ArgumentNullException(nameof(signer));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public SignedDataResponse<T> Build<T>(T responseDto)
        {
            var jsonString = _jsonSerializer.Serialize(responseDto);
            var response = new SignedDataResponse<T>
            {
                Payload = Base64.Encode(jsonString), Signature = GetSignatureB64(jsonString)
            };
            return response;
        }

        private string GetSignatureB64(string payload)
        {
            var payloadBytes = Encoding.UTF8.GetBytes(payload);
            var signatureBytes = _signer.GetSignature(payloadBytes);
            return Convert.ToBase64String(signatureBytes);
        }
    }
}