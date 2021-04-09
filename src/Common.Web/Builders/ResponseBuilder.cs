// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Text;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Signing;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Models;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Builders
{
    public class ResponseBuilder : IResponseBuilder
    {
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IContentSigner _signer;

        public ResponseBuilder(IJsonSerializer jsonSerializer, IContentSigner signer)
        {
            _signer = signer ?? throw new ArgumentNullException(nameof(signer));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public object Build<T>(T responseDto) where T : class
        {
            if (responseDto == null) throw new ArgumentNullException(nameof(responseDto));

            var jsonString = _jsonSerializer.Serialize(responseDto);
            var response = new SignedDataWrapper<T>
            {
                Payload = Base64.Encode(jsonString),
                Signature = GetSignatureB64(jsonString)
            };

            return response;
        }

        private string GetSignatureB64(string payload)
        {
            if (string.IsNullOrWhiteSpace(payload)) throw new ArgumentException(nameof(payload));

            var payloadBytes = Encoding.UTF8.GetBytes(payload);
            var signatureBytes = _signer.GetSignature(payloadBytes, true);
            return Convert.ToBase64String(signatureBytes);
        }
    }
}
