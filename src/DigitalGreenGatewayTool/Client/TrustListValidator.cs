// // Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// // Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// // SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Certificates;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Client
{
    public class TrustListValidator
    {
        private readonly ICertificateProvider _taCertProvider;

        public TrustListValidator(ICertificateProvider taCertProvider)
        {
            _taCertProvider = taCertProvider ?? throw new ArgumentNullException(nameof(taCertProvider));
        }

        // TODO structure the data correctly and the algorithm falls together
        //private void Structure(IList<TrustListDto> trustList)
        //{
        //    var d = new Dictionary<string, Dictionary<CertificateType, IList<TrustListDto>>>();

        //    foreach (var item in trustList)
        //    {
        //        if (!d.ContainsKey(item.Country))
        //            d[item.Country] = new Dictionary<CertificateType, IList<TrustListDto>>();

        //        if (!d[item.Country].ContainsKey(item.CertificateType))
        //            d[item.Country][item.CertificateType] = new List<TrustListDto>();

        //        d[item.Country][item.CertificateType].Add(item);
        //    }
        //}

        public IList<TrustListDto> Validate(IReadOnlyList<TrustListDto> trustList, IList<string> failures)
        {
            var invalids = new HashSet<string>();
            var validDgc = new List<TrustListDto>();

            var countries = trustList.Select(_ => _.Country).Distinct();

            foreach (var country in countries)
            {
                var uploadItem = trustList
                                .Where(_ => _.Country == country)
                                .FirstOrDefault(_ => _.CertificateType == CertificateType.Upload);

                if (uploadItem == null)
                {
                    failures.Add($"No Authentication certificate was found for {country}");

                    continue;
                }

                if (!ValidateSignature(uploadItem))
                {
                    failures.Add($"Authentication certificate for {country} was not signed by the DGCG TrustAnchor");
                    invalids.Add(uploadItem.Kid);
                }

                var cscaItems = trustList
                               .Where(_ => _.Country == country)
                               .Where(_ => _.CertificateType == CertificateType.Csca);

                var uploadCert = uploadItem.GetCertificate();

                var validCsca = new List<TrustListDto>();
                foreach (var cscaItem in cscaItems)
                {
                    if (!ValidateSignature(cscaItem, uploadCert))
                    {
                        failures.Add($"Authentication certificate for {country} was not signed by the DGCG TrustAnchor");
                        invalids.Add(uploadItem.Kid);
                    }

                    validCsca.Add(cscaItem);
                }

                // DGCs
                var dgcItems = trustList.Where(_ => _.Country == country && _.CertificateType == CertificateType.Dsc);
                foreach (var dgcItem in dgcItems)
                {
                    // Check that the key was issued by a valid CSCA for the country
                    var aki = dgcItem.GetCertificate().AuthorityKeyIdentifier();
                    var cs = validCsca.FirstOrDefault(_ => _.GetCertificate().SubjectKeyIdentifier() == aki);
                    if (cs == null)
                    {
                        failures.Add($"DSC {dgcItem.Kid}: CSCA with SKI {aki} not found for {country}");
                        continue;
                    }

                    // Check the sig
                    if (!ValidateSignature(dgcItem, uploadCert))
                    {
                        failures.Add($"DSC {dgcItem.Kid}: not signed with the Upload certificate for {country}");
                        invalids.Add(dgcItem.Kid);
                    }

                    validDgc.Add(dgcItem);
                }
            }

            return validDgc;
        }

        private bool ValidateSignature(TrustListDto item)
        {
            return ValidateSignature(item, _taCertProvider.GetCertificate());
        }

        private static bool ValidateSignature(TrustListDto item, X509Certificate2 certificate)
        {
            var contentInfo = new ContentInfo(item.GetCertificateBytes());
            var signedCms = new SignedCms(contentInfo, true);
            signedCms.Certificates.Add(certificate);
            signedCms.Decode(item.GetSignature());

            try
            {
                signedCms.CheckSignature(true);
            }
            catch (CryptographicException)
            {
                return false;
            }

            return true;
        }
    }
}
