// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Client;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Formatters
{
    /// <summary>
    ///     Outputs JSON in format defined by Tomas for this project
    /// </summary>
    public class DutchFormatter : ITrustListFormatter
    {
        private readonly IJsonSerializer _jsonSerializer;

        public DutchFormatter(IJsonSerializer serializer)
        {
            _jsonSerializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        private static IDictionary<string, string> OidName => new AttributeDictionary
            {{"1.3.6.1.4.1.0.1847.2021.1.1", "t"}, {"1.3.6.1.4.1.0.1847.2021.1.2", "v"}, {"1.3.6.1.4.1.0.1847.2021.1.3", "r"}};

        public string Format(IEnumerable<TrustListItem> trustList)
        {
            var resultSet = new Dictionary<string, List<DutchFormatItem>>();

            foreach (var item in trustList)
            {
                var certificate = DotNetUtilities.FromX509Certificate(new X509Certificate2(item.GetCertificate()));

                var resultItem = new DutchFormatItem
                {
                    SubjectPublicKey =
                        Convert.ToBase64String(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(certificate.GetPublicKey()).GetDerEncoded()),
                    KeyUsage = new List<string>()
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
                    resultSet.Add(item.Kid, new List<DutchFormatItem> {resultItem});
            }

            return _jsonSerializer.Serialize(resultSet);
        }
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "CollectionNeverQueried.Global")]
    internal class DutchFormatItem
    {
        [JsonPropertyName("subjectPk")] public string SubjectPublicKey { get; set; }

        [JsonPropertyName("keyUsage")] public IList<string> KeyUsage { get; set; }
    }
}
