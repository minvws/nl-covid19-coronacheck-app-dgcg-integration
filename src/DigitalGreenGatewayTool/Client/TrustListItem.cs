// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Security;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Client;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class TrustListItem
{
    private static readonly ILogger Log = ApplicationLogging.CreateLogger<TrustListItem>();

    private X509Certificate2 _cert;

    private byte[] _certBytes;

    private byte[] _sigBytes;
    [JsonPropertyName("kid")] public string Kid { get; set; }

    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }

    [JsonPropertyName("country")] public string Country { get; set; }

    [JsonPropertyName("certificateType")] public CertificateType CertificateType { get; set; }

    [JsonPropertyName("thumbprint")] public string Thumbprint { get; set; }

    // Base64 encoded CMS signature of certificate
    [JsonPropertyName("signature")] public string Signature { get; set; }

    // Base64 encoded certificate
    [JsonPropertyName("rawData")] public string RawData { get; set; }

    public byte[] GetSignature()
    {
        return _sigBytes ?? throw new InvalidOperationException("Signature must be Parsed before it can be accessed");
    }

    public X509Certificate2 GetCertificate()
    {
        return _cert ?? throw new InvalidOperationException("Certificate must be Parsed before it can be accessed");
    }

    public byte[] GetCertificateBytes()
    {
        return _certBytes ?? throw new InvalidOperationException("Certificate must be Parsed before it can be accessed");
    }

    public void ParseCertificate()
    {
        _certBytes = Base64.Decode(RawData);
        _cert = new X509Certificate2(_certBytes);
    }

    public void ParseSignature()
    {
        _sigBytes = Base64.Decode(Signature);
    }

    public bool ValidateSignature(IEnumerable<X509Certificate2> certificates)
    {
        return certificates.Any(ValidateSignature);
    }

    public bool ValidateSignature(X509Certificate2 certificate)
    {
        try
        {
            var contentInfo = new ContentInfo(GetCertificateBytes());
            var signedCms = new SignedCms(contentInfo, true);
            signedCms.Certificates.Add(certificate);
            signedCms.Decode(GetSignature());
            signedCms.CheckSignature(true);
            return true;
        }
        catch (Exception e)
        {
            // ReSharper disable once StringLiteralTypo
            Log.LogError(e, "Error validating signature with System.Security.Cryptography.Pkcs");

            return ValidateBouncy(certificate, GetSignature(), GetCertificateBytes());
        }
    }

    private static bool ValidateBouncy(X509Certificate2 certificate, byte[] signature, byte[] content)
    {
        if (certificate == null) throw new ArgumentNullException(nameof(certificate));
        if (content == null) throw new ArgumentNullException(nameof(content));
        if (signature == null) throw new ArgumentNullException(nameof(signature));

        try
        {
            var bouncyCertificate = DotNetUtilities.FromX509Certificate(certificate);
            var publicKey = bouncyCertificate.GetPublicKey();
            var cms = new CmsSignedData(new CmsProcessableByteArray(content), signature);

            var result = cms.GetSignerInfos()
                            .GetSigners()
                            .Cast<SignerInformation>()
                            .Select(signer => signer.Verify(publicKey)).FirstOrDefault();

            return result;
        }
        catch (Exception e)
        {
            Log.LogError(e, "Error validating signature with BouncyCastle");

            return false;
        }
    }
}
