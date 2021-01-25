// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.AspNetCore.Mvc;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Extensions;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Models;
using System;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Controllers
{
    [ApiController]
    [Route("proof")]
    public class ProofOfTestController : ControllerBase
    {
        private readonly IProofOfTestService _potService;
        private readonly IUtcDateTimeProvider _dateTimeProvider;

        public ProofOfTestController(IProofOfTestService potService, IUtcDateTimeProvider dateTimeProvider)
        {
            _potService = potService ?? throw new ArgumentNullException(nameof(potService));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        /// <summary>
        /// Issues the proof of (negative) test
        /// </summary>
        [HttpPost]
        [Route("issue")]
        public IssueProofResult IssueProof(IssueProofRequest request)
        {
            var dateTime = _dateTimeProvider.Now().ToGoApiString();
            var proof = _potService.GetProofOfTest(request.TestType, dateTime, request.Nonce);

            return new IssueProofResult {Proof = proof};
        }

        /// <summary>
        /// Generates and returns a random nonce
        /// </summary>
        [HttpGet]
        [Route("nonce")]
        public GenerateNonceResult GenerateNonce()
        {
            var nonce = _potService.GenerateNonce();

            return new GenerateNonceResult {Nonce = nonce};
        }
    }
}