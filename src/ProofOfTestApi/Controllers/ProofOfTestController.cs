// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.AspNetCore.Mvc;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Extensions;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.IssuerInterop;
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
        private readonly IJsonSerializer _jsonSerializer;

        public ProofOfTestController(IProofOfTestService potService, IUtcDateTimeProvider dateTimeProvider, IJsonSerializer jsonSerializer)
        {
            _potService = potService ?? throw new ArgumentNullException(nameof(potService));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        /// <summary>
        /// Issues the proof of (negative) test
        /// </summary>
        [HttpPost]
        [Route("issue")]
        [ProducesResponseType(typeof(IssueProofResult), 200)]
        public IActionResult IssueProof(IssueProofRequest request)
        {
            var dateTime = _dateTimeProvider.Now().ToGoApiString();

            var commitmentsJson = Base64.Decode(request.Commitments);

            try
            {
                var proofResult = _potService.GetProofOfTest(request.TestType, dateTime, request.Nonce, commitmentsJson);

                var result = _jsonSerializer.Deserialize<IssueProofResult>(proofResult);

                return Ok(result);
            }
            catch (IssuerException)
            {
                // todo IssuerException as urgent/actionable
                // todo JsonDeserialized exception as urgent/actionable

                return StatusCode(500);
            }
        }

        /// <summary>
        /// Generates and returns a random nonce
        /// </summary>
        [HttpGet]
        [Route("nonce")]
        [ProducesResponseType(typeof(GenerateNonceResult), 200)]
        public IActionResult GenerateNonce()
        {
            var nonce = _potService.GenerateNonce();

            return Ok(new GenerateNonceResult {Nonce = nonce});
        }
    }
}