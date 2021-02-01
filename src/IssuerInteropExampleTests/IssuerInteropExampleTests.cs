// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Reflection;
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
            var attributes = new string[] {"attributes", "tested"};

            var expected = issuerPkXml + "|" + issuerSkXml + "|" + issuerNonceB64 + "|" + commitmentsJson + "|" + attributes[0];
            
            var result = Issuer.IssueProof(issuerPkXml, issuerSkXml, issuerNonceB64, commitmentsJson, attributes);

            Assert.Equal(result, expected);
        }
    }
}
