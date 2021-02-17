// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Signing;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Models;
using System;
using System.Text;

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

        //TODO could constrain to class? If struct are returned, null check is superfluous but would cause no side effects
        public SignedDataResponse<T> Build<T>(T responseDto)
        {
            if (responseDto == null) throw new ArgumentNullException(nameof(responseDto));

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