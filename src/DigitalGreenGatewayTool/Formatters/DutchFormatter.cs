// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        public string Format(IEnumerable<TrustListItem> trustList)
        {
            var resultSet = new Dictionary<string, DutchFormatItem>();

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
                        resultItem.KeyUsage.Add(usage.ToString());
                    }

                resultSet.Add(item.Kid, resultItem);
            }

            return JsonSerializer.Serialize(resultSet);
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
