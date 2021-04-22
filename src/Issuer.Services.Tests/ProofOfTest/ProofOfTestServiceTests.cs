using Moq;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.Attributes;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.Keystores;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.PartialIssuance;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.ProofOfTest;
using NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerInterop;
using Xunit;

namespace Issuer.Services.Tests.ProofOfTest
{
    public class ProofOfTestServiceTests
    {
        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void EnablePartialIssuanceForDynamicProof_setting_works(bool enabled, bool expectedResult)
        {
            var issuanceServiceCalled = false;

            // Assemble
            var keystore = new Mock<IKeyStore>();
            keystore.Setup(_ => _.GetKeys(It.IsAny<string>())).Returns(new KeySet());
            var issuer = new Mock<IIssuerInterop>();
            issuer.Setup(_ => _.IssueProof(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                                           It.IsAny<string>()));
            var partialIssuerService = new Mock<IPartialIssuanceService>();
            partialIssuerService.Setup(_ => _.Apply(It.IsAny<ProofOfTestAttributes>())).Callback(() => issuanceServiceCalled = true);

            var config = new Mock<IProofOfTestServiceConfig>();
            config.Setup(_ => _.EnablePartialIssuanceForDynamicProof).Returns(enabled);

            var potService = new ProofOfTestService(new StandardJsonSerializer(), keystore.Object, issuer.Object, partialIssuerService.Object, config.Object);

            // Act
            potService.GetProofOfTest(new ProofOfTestAttributes(), ".", ".", ".");

            // Assert
            Assert.Equal(expectedResult, issuanceServiceCalled);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void EnablePartialIssuanceForStaticProof_setting_works(bool enabled, bool expectedResult)
        {
            var issuanceServiceCalled = false;

            // Assemble
            var keystore = new Mock<IKeyStore>();
            keystore.Setup(_ => _.GetKeys(It.IsAny<string>())).Returns(new KeySet());
            var issuer = new Mock<IIssuerInterop>();
            issuer.Setup(_ => _.IssueStaticDisclosureQr(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            var partialIssuerService = new Mock<IPartialIssuanceService>();
            partialIssuerService.Setup(_ => _.Apply(It.IsAny<ProofOfTestAttributes>())).Callback(() => issuanceServiceCalled = true);

            var config = new Mock<IProofOfTestServiceConfig>();
            config.Setup(_ => _.EnablePartialIssuanceForStaticProof).Returns(enabled);

            var potService = new ProofOfTestService(new StandardJsonSerializer(), keystore.Object, issuer.Object, partialIssuerService.Object, config.Object);

            // Act
            potService.GetStaticProofQr(new ProofOfTestAttributes(), ".");

            // Assert
            Assert.Equal(expectedResult, issuanceServiceCalled);
        }
    }
}
