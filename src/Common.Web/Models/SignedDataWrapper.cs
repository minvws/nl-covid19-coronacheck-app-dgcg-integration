// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Validation;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Models
{
    public class SignedDataWrapper<T>
    {
        [JsonPropertyName("payload")]
        [Base64String]
        public string? Payload { get; set; }

        [JsonPropertyName("signature")]
        [Base64String]
        public string? Signature { get; set; }

        [JsonIgnore] public byte[] PayloadBytes => Convert.FromBase64String(Payload ?? string.Empty);

        [JsonIgnore] public byte[] SignatureBytes => Convert.FromBase64String(Signature ?? string.Empty);

        /// <summary>
        ///     Unpacks Payload as T.
        /// </summary>
        [return: MaybeNull]
        public T Unpack(IJsonSerializer serializer)
        {
            if (string.IsNullOrWhiteSpace(Payload)) return default;

            var payloadString = Base64.Decode(Payload);

            return serializer.Deserialize<T>(payloadString);
        }
    }
}
