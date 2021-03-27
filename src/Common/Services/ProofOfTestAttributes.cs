// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Extensions;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class ProofOfTestAttributes
    {
        public ProofOfTestAttributes(
            DateTime sampleTime,
            string testType,
            string firstNameInitial,
            string lastNameInitial,
            string birthDay,
            string birthMonth,
            bool isPaperProof = false,
            bool isSpecimen = false)
        {
            SampleTime = sampleTime.ToUnixTime().ToString();
            TestType = testType;
            FirstNameInitial = firstNameInitial;
            LastNameInitial = lastNameInitial;
            BirthDay = birthDay;
            BirthMonth = birthMonth;
            IsPaperProof = isPaperProof ? "1" : "0";
            IsSpecimen = isSpecimen ? "1" : "0";
        }

        /// <summary>
        ///     SampleTime encoded as a unix
        /// </summary>
        [JsonPropertyName("sampleTime")]
        public string SampleTime { get; }

        /// <summary>
        ///     Well known string identifying the type of test.
        /// </summary>
        [JsonPropertyName("testType")]
        public string TestType { get; }

        /// <summary>
        ///     Flag to show whether it's a specimen ("1") or not ("0")
        /// </summary>
        [JsonPropertyName("isPaperProof")]
        public string IsPaperProof { get; }

        /// <summary>
        ///     Flag to show whether it's a specimen ("1") or not ("0")
        /// </summary>
        [JsonPropertyName("isSpecimen")]
        public string IsSpecimen { get; }

        /// <summary>
        ///     A-Z{1}
        /// </summary>
        [JsonPropertyName("firstNameInitial")]
        public string FirstNameInitial { get; }

        /// <summary>
        ///     A-Z{1}
        /// </summary>
        [JsonPropertyName("lastNameInitial")]
        public string LastNameInitial { get; }

        /// <summary>
        ///     1-31 or X
        /// </summary>
        [JsonPropertyName("birthDay")]
        public string BirthDay { get; }

        /// <summary>
        ///     1-12 or X
        /// </summary>
        [JsonPropertyName("birthMonth")]
        public string BirthMonth { get; }
    }
}
