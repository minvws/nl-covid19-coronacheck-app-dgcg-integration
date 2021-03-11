// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Validation;
using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Models
{
    public class SignedDataResponse<T>
    {
        [JsonPropertyName("payload")]
        [Base64String]
        public string Payload { get; set; }

        [JsonPropertyName("signature")]
        [Base64String]
        public string Signature { get; set; }

        /// <summary>
        /// Unpacks Payload as T.
        /// </summary>
        public T Unpack(IJsonSerializer serializer)
        {
            if (string.IsNullOrWhiteSpace(Payload)) return default;

            var payloadString = Base64.Decode(Payload);
            
            return serializer.Deserialize<T>(payloadString);
        }
    }
}
