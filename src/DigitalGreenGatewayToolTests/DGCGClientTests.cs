// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Linq;
using Moq;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Certificates;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Config;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Signing;
using NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Client;
using Xunit;

namespace DigitalGreenGatewayToolTests;

/// <summary>
///     These are integration tests, they require a local DGCG server configured with only our own keys as per
///     the guide from readme of the DGCG.
/// </summary>
public class DgcgClientTests
{
    [Fact]
    public async void Tests_that_GetTrustList_works_with_local_DGCG_server()
    {
        var config = new Config();
        var authCertProvider = new FileSystemCertificateProvider(config);
        var signer = new Mock<IContentSigner>().Object;
        IDgcgClient client = new DgcgClient(config, new StandardJsonSerializer(), authCertProvider, signer);

        var result = await client.GetTrustList();

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(3, result.Count);
    }

    [Theory]
    [InlineData(CertificateType.Authentication)]
    [InlineData(CertificateType.Csca)]
    [InlineData(CertificateType.Dsc)]
    [InlineData(CertificateType.Upload)]
    public async void Tests_that_GetTrustList_CertificateType_works_with_local_DGCG_server(CertificateType certificateType)
    {
        var config = new Config();
        var authCertProvider = new FileSystemCertificateProvider(config);
        var signer = new Mock<IContentSigner>().Object;
        IDgcgClient client = new DgcgClient(config, new StandardJsonSerializer(), authCertProvider, signer);

        var result = await client.GetTrustList(certificateType);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(1, result.Count);
        var item = result.First();
        Assert.Equal(certificateType, item.CertificateType);
    }

    [Theory]
    [InlineData(CertificateType.Authentication, "NL", 1)]
    [InlineData(CertificateType.Csca, "NL", 1)]
    [InlineData(CertificateType.Dsc, "NL", 0)]
    [InlineData(CertificateType.Upload, "NL", 1)]
    [InlineData(CertificateType.Authentication, "DE", 0)]
    [InlineData(CertificateType.Csca, "DE", 0)]
    [InlineData(CertificateType.Dsc, "DE", 0)]
    [InlineData(CertificateType.Upload, "DE", 0)]
    public async void Tests_that_GetTrustList_CertificateType_Land_works_with_local_DGCG_server(
        CertificateType certificateType, string land, int expectedCount)
    {
        var config = new Config();
        var authCertProvider = new FileSystemCertificateProvider(config);
        var signer = new Mock<IContentSigner>().Object;
        IDgcgClient client = new DgcgClient(config, new StandardJsonSerializer(), authCertProvider, signer);

        var result = await client.GetTrustList(certificateType, land);

        Assert.NotNull(result);

        if (expectedCount <= 0) return;

        Assert.NotEmpty(result);
        Assert.Equal(expectedCount, result.Count);
        var item = result.First();
        Assert.Equal(certificateType, item.CertificateType);
        Assert.Equal(land, item.Country);
    }
}

public class Config : IDgcgClientConfig, ICertificateLocationConfig
{
    public bool UseEmbedded => false;
    public string Path => "auth.pfx";
    public string Password => string.Empty;
    public bool SendAuthenticationHeaders => true;
    public string GatewayUrl => "http://localhost:8080";
    public bool IncludeChainInSignature => false;
    public bool IncludeCertsInSignature => false;
}
