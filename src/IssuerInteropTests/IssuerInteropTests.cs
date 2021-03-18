// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Testing;
using NL.Rijksoverheid.CoronaTester.BackEnd.IssuerInterop;
using System;
using System.Text.Json;
using Xunit;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.IssuerInteropTests
{
    public class IssuerInteropTests
    {
        [Fact]
        public void TestsGenerateNonce()
        {
            var issuerPkId = "testPk";
            var issuer = new Issuer();
            var result = issuer.GenerateNonce(issuerPkId);
            Assert.True(IsBase64String(result));
        }

        [Fact]
        public void TestsIssueProof()
        {
            var issuer = new Issuer();
            var keystore = new AssemblyKeyStore(new TestLogger<AssemblyKeyStore>());

            var attributes = new ProofOfTestAttributes(DateTime.UtcNow.ToString("o"), "PCR", "A", "A", "1", "1");

            var issuerPkId = "testPk";
            var issuerPkXml = keystore.GetPublicKey();
            var issuerSkXml = keystore.GetPrivateKey();
            var issuerNonceB64 = issuer.GenerateNonce(issuerPkId);
            var attributesJson = JsonSerializer.Serialize(attributes);
            var commitments = @"{""n_2"":""3Z0EabX/T+jFK7VyGxWfkQ=="",""combinedProofs"":[{""U"":""WpKqkB9KfkQS4dWq98BwUaXXyzHGjuAbX0EbGThYy8TUWv4BfgA9bmueTOsjVWhtmpBLjkBBrrowaLdaqJGzV5zaPnR+jgHtKT9A3IwfZnCBIYivzsWnqjbox0vrhaRIMa4ESNVpcsCNx2N/GcrXvs1T62kklLLxM3K/ziKJiTdTrgt9vCZ3M5LJuYIO443UiANMEWj2vWdV30JIuGNW5UKL8U6oxK0E+m9+8IxP/RbFrTJuL7jGzEqiqsfArLvAgQJyWsgpGHFQ5pl3RzQscxh3PZxti8qmnyke94mbzazahgbbZQimAbSiqe56kwrgO5UK8OrkmdsWqNSzVFG2yA=="",""c"":""pSiu2TrnziN77RqKPFZ68p9pE/V8LkU0kvFTWOtYBpg="",""v_prime_response"":""igaPjUTzX6WD2Js934D/YX2BB0mJz+n+M+WtrrALtU5guAHdS2BiTde5/KDRSkwLve7mcqNGIwg7nb+YLJxg1MKY68q1OA3CaFnvRobdRBfAIfys290p9Nw5zaEuN0FcH+f4BxXuZcwX3BC7W0nRTMPVeo31vFjtePcBi54diNKr5AnZmFSYXE0tL3kgcUZr1pr+54nuE0AshwoLpmvLB8URXI06HC2X3k/n9XDn3UuJlDsSaVWH87qluxn2+H3OGOWaxqw1h2M8ns+lG6Rit4vcGmx1z34Nq6jhJusK46xiSIirzi+l6/oqKxRWHsNmBD9AtZf4OYY7L/d43Vn4QPuA0uXv8GmOutgnuzf1vl9ltJ7dD3v6u1FReLa5fbQ54HKghv6LbRz1g21WAAGdwtvqJAvjZa6EuZDx0o5VkdY="",""s_response"":""kPKLjTmLCXAYZ4XmvgRQ8US+f1SowzywaovNu5abnaRJAz4xs2tuKWUdVkZnyhCHOgl1LXRvG6Aw/6bteFh/5M/FA5NWQjzdajQ=""}]}";

            var result = issuer.IssueProof(issuerPkId, issuerPkXml, issuerSkXml, issuerNonceB64, commitments, attributesJson);

            // Result contains a couple of expected strings
            Assert.Contains("proof", result);
            Assert.Contains("e_response", result);
            Assert.Contains("signature", result);
            Assert.Contains("KeyshareP", result);
        }

        [Fact]
        public void TestsIssueStaticDisclosureQr()
        {
            var issuer = new Issuer();
            var keystore = new AssemblyKeyStore(new TestLogger<AssemblyKeyStore>());

            var attributes = new ProofOfTestAttributes(DateTime.UtcNow.ToString("o"), "PCR", "A", "A", "1", "1");

            var issuerPkId = "testPk";
            var issuerPkXml = keystore.GetPublicKey();
            var issuerSkXml = keystore.GetPrivateKey();
            var attributesJson = JsonSerializer.Serialize(attributes);

            var result = issuer.IssueStaticDisclosureQr(issuerPkId, issuerPkXml, issuerSkXml, attributesJson);

            // Result contains something
            Assert.NotEmpty(result);
        }

        private static bool IsBase64String(string data)
        {
            try
            {
                Convert.FromBase64String(data);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
