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
        /// String representing (UUID) of the test type.
        /// </summary>
        [Required]
        [JsonPropertyName("testType")]
        public string TestType { get; set; }

        /// <summary>
        /// Unix time for when the test sample was taken
        /// </summary>
        [Required]
        [JsonPropertyName("sampleTime")]
        public string SampleTime { get; set; }

        /// <summary>
        /// Commitments bytes formatted as a base64 string.
        /// </summary>
        [Required]
        [Base64String]
        [JsonPropertyName("commitments")]
        public string Commitments { get; set; }

        /// <summary>
        /// SessionToken.
        /// </summary>
        [Required]
        [JsonPropertyName("sessionToken")]
        public string SessionToken { get; set; }

        /// <summary>
        /// Test result received from the test provider.
        /// </summary>
        [Required]
        [JsonPropertyName("testResult")]
        public SignedDataResponse<TestResult> TestResult { get; set; }
        
        #region Unpacked object

        // This section will be replaced with a model binder eventually, for now it's OK

        [JsonIgnore] public TestResult UnpackedTestResult { get; set; }

        [JsonIgnore] public IssuerCommitmentMessage UnpackedCommitments { get; set; }

        public bool UnpackAll(IJsonSerializer serializer)
        {
            try
            {
                UnpackedTestResult = TestResult.Unpack(serializer);
                UnpackedCommitments = serializer.Deserialize<IssuerCommitmentMessage>(Commitments);
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
                UnpackedTestResult.ProviderIdentifier, 
                TestResult.PayloadBytes,
                TestResult.SignatureBytes);
        }
    }
}
