// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Builders;
using NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Client;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Formatters
{
    /// <summary>
    ///     Outputs JSON in format defined by Tomas for this project
    /// </summary>
    public class DutchFormatter : ITrustListFormatter
    {
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IResponseBuilder _responseBuilder;

        public DutchFormatter(IJsonSerializer serializer, IResponseBuilder responseBuilder)
        {
            _jsonSerializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _responseBuilder = responseBuilder ?? throw new ArgumentNullException(nameof(responseBuilder));
        }

        private static IDictionary<string, string> OidName => new AttributeDictionary
            { { "1.3.6.1.4.1.0.1847.2021.1.1", "t" }, { "1.3.6.1.4.1.0.1847.2021.1.2", "v" }, { "1.3.6.1.4.1.0.1847.2021.1.3", "r" } };

        public string Format(IEnumerable<TrustListItem> trustList, string thirdPartyKeyFile)
        {
            var resultSet = new Dictionary<string, List<DutchFormatItem>>();

            foreach (var item in trustList)
            {
                var certificate = DotNetUtilities.FromX509Certificate(new X509Certificate2(item.GetCertificate()));

                var resultItem = new DutchFormatItem
                {
                    SubjectPublicKey =
                        Convert.ToBase64String(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(certificate.GetPublicKey()).GetDerEncoded()),
                    KeyUsage = new List<string>(),
                    Country = item.Country,
                    Land = GetSubjectAlternativeNameLandCode(certificate)
                };

                var extendedKeyUsage = certificate.GetExtendedKeyUsage();
                if (extendedKeyUsage != null)
                    foreach (var usage in certificate.GetExtendedKeyUsage())
                    {
                        if (usage == null) continue;
                        var usageOid = usage.ToString() ?? string.Empty;
                        if (OidName.ContainsKey(usageOid)) resultItem.KeyUsage.Add(OidName[usageOid]);
                    }

                if (resultSet.ContainsKey(item.Kid))
                    resultSet[item.Kid].Add(resultItem);
                else
                    resultSet.Add(item.Kid, new List<DutchFormatItem> { resultItem });
            }

            if (!string.IsNullOrWhiteSpace(thirdPartyKeyFile)) AddThirdPartyKeys(resultSet, thirdPartyKeyFile);

            return _jsonSerializer.Serialize(_responseBuilder.Build(resultSet));
        }

        private void AddThirdPartyKeys(IDictionary<string, List<DutchFormatItem>> resultSet, string path)
        {
            var thirdPartyKeysText = File.ReadAllText(path);
            var thirdPartyKeys = _jsonSerializer.Deserialize<List<ThirdPartyKey>>(thirdPartyKeysText);

            var countryCode = Path.GetFileName(path)[..2];

            foreach (var item in thirdPartyKeys)
            {
                var resultItem = new DutchFormatItem
                {
                    Country = countryCode,
                    SubjectPublicKey = item.PublicKey,
                    KeyUsage = new List<string>(),
                    Land = ""
                };

                if (resultSet.ContainsKey(item.Kid))
                    resultSet[item.Kid].Add(resultItem);
                else
                    resultSet.Add(item.Kid, new List<DutchFormatItem> { resultItem });
            }
        }

        private static string GetSubjectAlternativeNameLandCode(X509Certificate cert)
        {
            var subjectAlternativeNames = cert.GetSubjectAlternativeNames();

            if (subjectAlternativeNames == null) return string.Empty;

            foreach (var item in cert.GetSubjectAlternativeNames())
            {
                if (item is not ArrayList { Count: 2 } itemCollection) continue;
                if (itemCollection[0] is not 4) continue;
                if (itemCollection[1] is not string value) continue;

                if (value.StartsWith("L=")) return value[2..];
            }

            return string.Empty;
        }
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "CollectionNeverQueried.Global")]
    internal class DutchFormatItem
    {
        [JsonPropertyName("subjectPk")] public string SubjectPublicKey { get; set; }

        [JsonPropertyName("ian")] public string Country { get; set; }

        [JsonPropertyName("san")] public string Land { get; set; }

        [JsonPropertyName("keyUsage")] public IList<string> KeyUsage { get; init; }
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "CollectionNeverQueried.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal class ThirdPartyKey
    {
        [JsonPropertyName("kid")] public string Kid { get; set; }

        [JsonPropertyName("publicKey")] public string PublicKey { get; set; }
    }
}
