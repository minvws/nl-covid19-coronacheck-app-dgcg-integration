// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Models;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Validation;
using NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Services;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Models
{
    public class IssueProofRequest
    {
        /// <summary>
        /// Commitments bytes formatted as a base64 string.
        /// </summary>
        [Required]
        [Base64String]
        [JsonPropertyName("icm")]
        public string Commitments { get; set; }

        /// <summary>
        /// SessionToken.
        /// </summary>
        [Required]
        [JsonPropertyName("stoken")]
        public string SessionToken { get; set; }

        /// <summary>
        /// Test result received from the test provider.
        /// </summary>
        [Required]
        [JsonPropertyName("test")]
        public SignedDataWrapper<TestResult> TestResult { get; set; }
        
        #region Unpacked object

        // TODO This section will be replaced with a model binder eventually, for now it's OK

        [JsonIgnore] public TestResult Test { get; set; }

        [JsonIgnore] public IssuerCommitmentMessage Icm { get; set; }

        public bool UnpackAll(IJsonSerializer serializer)
        {
            try
            {
                Test = TestResult.Unpack(serializer);
                Icm = serializer.Deserialize<IssuerCommitmentMessage>(Commitments);
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion

        public bool ValidateSignature(ITestProviderSignatureValidator signatureValidator)
        {
            return signatureValidator.Validate(
                Test.ProviderIdentifier, 
                TestResult.PayloadBytes,
                TestResult.SignatureBytes);
        }
    }
}
