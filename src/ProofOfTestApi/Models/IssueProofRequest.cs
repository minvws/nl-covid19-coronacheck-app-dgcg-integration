// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Models;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Signatures;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Validation;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.ProofOfTestApi.Models
{
    public class IssueProofRequest
    {
        /// <summary>
        ///     Commitments bytes formatted as a base64 string.
        /// </summary>
        [Required]
        [Base64String]
        [JsonPropertyName("icm")]
        public string? Commitments { get; set; }

        /// <summary>
        ///     SessionToken.
        /// </summary>
        [Required]
        [JsonPropertyName("stoken")]
        public string? SessionToken { get; set; }

        /// <summary>
        ///     Test result received from the test provider.
        /// </summary>
        [Required]
        [JsonPropertyName("test")]
        public SignedDataWrapper<TestResult>? TestResult { get; set; }

        public bool ValidateSignature(ITestProviderSignatureValidator signatureValidator)
        {
            if (signatureValidator == null) throw new ArgumentNullException(nameof(signatureValidator));
            if (Test?.ProviderIdentifier == null) return false;
            if (TestResult?.PayloadBytes == null) return false;

            return signatureValidator.Validate(
                Test.ProviderIdentifier,
                TestResult.PayloadBytes,
                TestResult.SignatureBytes);
        }

        #region Unpacked object

        // TODO This section will be replaced with a model binder eventually, for now it's OK

        [JsonIgnore] public TestResult? Test { get; private set; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        [JsonIgnore] public IssuerCommitmentMessage? Icm { get; set; }

        public bool UnpackAll(IJsonSerializer serializer)
        {
            if (TestResult == null) return false;
            if (Commitments == null) return false;

            try
            {
                var testResult = TestResult.Unpack(serializer);
                if (testResult == null) return false;
                Test = testResult;
                var icmString = Base64.Decode(Commitments);
                Icm = serializer.Deserialize<IssuerCommitmentMessage>(icmString);
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
