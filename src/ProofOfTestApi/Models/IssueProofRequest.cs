// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.ComponentModel.DataAnnotations;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Validation;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Models
{
    public class IssueProofRequest
    {
        /// <summary>
        /// String representing the type of test, valid values are: [TBC]
        /// </summary>
        [Required]
        public string TestType { get; set; }
        
        /// <summary>
        /// Nonce bytes formatted as a base64 string.
        /// </summary>
        [Required]
        //[Base64String]
        public string Nonce { get; set; }

        /// <summary>
        /// Commitments bytes formatted as a base64 string.
        /// </summary>
        [Required]
        [Base64String]
        public string Commitments { get; set; }
    }
}