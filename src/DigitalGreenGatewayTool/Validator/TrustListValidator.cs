// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Certificates;
using NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Client;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Validator;

public class TrustListValidator
{
    private static readonly ILogger Log = ApplicationLogging.CreateLogger<TrustListValidator>();
    private readonly ICertificateProvider _taCertProvider;

    public TrustListValidator(ICertificateProvider taCertProvider)
    {
        _taCertProvider = taCertProvider ?? throw new ArgumentNullException(nameof(taCertProvider));
    }

    public TrustListValidatorResult Validate(IEnumerable<TrustListItem> trustList)
    {
        var result = new TrustListValidatorResult();

        var nestedTrustList = new Dictionary<string, Dictionary<CertificateType, IList<TrustListItem>>>();

        var trustAnchorCert = _taCertProvider.GetCertificate();

        // Create nested trust list
        foreach (var item in trustList)
        {
            // Properly initialise the items, removing any which are clearly not valid
            try
            {
                item.ParseCertificate();
            }
            catch (Exception e)
            {
                const string msg = "RawData does not contain a valid certificate.";
                result.AddInvalid(item, msg);
                Log.LogError(e, msg);

                continue;
            }

            try
            {
                item.ParseSignature();
            }
            catch (Exception e)
            {
                const string msg = "Signature valid base64 string.";
                result.AddInvalid(item, msg);
                Log.LogError(e, msg);

                continue;
            }

            // Fill nested structure
            if (!nestedTrustList.ContainsKey(item.Country))
                nestedTrustList[item.Country] = new Dictionary<CertificateType, IList<TrustListItem>>
                {
                    [CertificateType.Csca] = new List<TrustListItem>(),
                    [CertificateType.Dsc] = new List<TrustListItem>(),
                    [CertificateType.Upload] = new List<TrustListItem>(),
                    [CertificateType.Authentication] = new List<TrustListItem>()
                };

            nestedTrustList[item.Country][item.CertificateType].Add(item);
        }

        foreach (var countryName in nestedTrustList.Keys)
        {
            //
            // Validate Upload certificates
            var uploadItems = nestedTrustList[countryName][CertificateType.Upload].ToList();
            var validUploadItems = new List<TrustListItem>();

            foreach (var uploadItem in uploadItems)
                if (uploadItem.ValidateSignature(trustAnchorCert))
                {
                    validUploadItems.Add(uploadItem);
                }
                else
                {
                    Log.LogWarning($"Invalid signature on the upload certificate for {countryName}. All of their certificates will be marked as invalid.");

                    const string msg = "Invalid signature on the Upload certificate.";

                    foreach (var item in nestedTrustList[countryName][CertificateType.Csca])
                        result.AddInvalid(item, msg);
                    foreach (var item in nestedTrustList[countryName][CertificateType.Dsc])
                        result.AddInvalid(item, msg);
                }

            if (!validUploadItems.Any())
            {
                Log.LogWarning($"No valid upload certificates found for {countryName}. All of their certificates will be marked as invalid.");

                const string msg = "No upload certificate(s) available for the country";

                foreach (var item in nestedTrustList[countryName][CertificateType.Csca])
                    result.AddInvalid(item, msg);
                foreach (var item in nestedTrustList[countryName][CertificateType.Dsc])
                    result.AddInvalid(item, msg);

                continue;
            }

            var uploadCertificates = validUploadItems.Select(_ => _.GetCertificate()).ToList();

            //
            // Validate CSCA certificate
            if (nestedTrustList[countryName][CertificateType.Csca].Count == 0)
            {
                Log.LogWarning($"No CSCA certificates found for {countryName}, all of their DSCs will be marked as invalid.");

                const string msg = "No CSCA certificates available for the country";

                foreach (var item in nestedTrustList[countryName][CertificateType.Csca])
                    result.AddInvalid(item, msg);
                foreach (var item in nestedTrustList[countryName][CertificateType.Dsc])
                    result.AddInvalid(item, msg);

                continue;
            }

            var validCscaCertificates = new Dictionary<string, List<X509Certificate2>>();
            foreach (var cscaItem in nestedTrustList[countryName][CertificateType.Csca])
            {
                if (!cscaItem.ValidateSignature(uploadCertificates)) result.AddInvalid(cscaItem, "Invalid upload certificate signature");

                var cscaCert = cscaItem.GetCertificate();

                if (cscaCert.SubjectName.Name == null)
                {
                    result.AddInvalid(cscaItem, "Invalid certificate: SubjectName missing");
                    continue;
                }

                if (!validCscaCertificates.ContainsKey(cscaCert.SubjectName.Name))
                    validCscaCertificates.Add(cscaCert.SubjectName.Name, new List<X509Certificate2>());

                // CSCA can be included multiple times, and multiple CSCA with the same DN but different key can exist
                validCscaCertificates[cscaCert.SubjectName.Name].Add(cscaCert);
            }

            //
            // Validate DSG
            foreach (var dsgItem in nestedTrustList[countryName][CertificateType.Dsc])
            {
                if (!dsgItem.ValidateSignature(uploadCertificates)) result.AddInvalid(dsgItem, "Invalid upload certificate signature");

                var dsgCert = dsgItem.GetCertificate();

                if (dsgCert.IssuerName.Name == null)
                {
                    result.AddInvalid(dsgItem, "Invalid certificate: IssuerName missing");
                    continue;
                }

                if (!validCscaCertificates.ContainsKey(dsgCert.IssuerName.Name))
                {
                    result.AddInvalid(dsgItem, "Issuing CSCA certificate not found");
                    continue;
                }

                result.AddValid(dsgItem);
            }
        }

        return result;
    }
}
