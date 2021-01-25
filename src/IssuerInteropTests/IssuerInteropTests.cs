// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaTester.BackEnd.IssuerInterop;
using Xunit;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.IssuerInteropTests
{
    public class IssuerInteropTests
    {
        [Fact]
        public void TestsGenerateNonce()
        {
            const string expected = "GenerateIssuerNonceB64 has generated!";
            var result = Issuer.GenerateNonce();

            Assert.Equal(result, expected);
        }

        [Fact]
        public void TestsIssueProof()
        {
            const string issuerPkXml = "issuerPkXml";
            const string issuerSkXml = "issuerSkXml";
            const string issuerNonceB64 = "issuerNonceB64";
            const string commitmentsJson = "{ infected: false }";

            const string expected = issuerPkXml + "|" + issuerSkXml + "|" + issuerNonceB64 + "|" + commitmentsJson;
            
            var result = Issuer.IssueProof(issuerPkXml, issuerSkXml, issuerNonceB64, commitmentsJson);

            Assert.Equal(result, expected);
        }
    }
}
