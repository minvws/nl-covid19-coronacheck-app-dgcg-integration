// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.ProofOfTestApi.Commands;
using NL.Rijksoverheid.CoronaCheck.BackEnd.ProofOfTestApi.Models;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.ProofOfTestApi.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("v{version:apiVersion}/test")]
    [ApiVersion("3")]
    public class ProofOfTestControllerV3 : ControllerBase
    {
        private readonly IJsonSerializer _jsonSerializer;

        public ProofOfTestControllerV3(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        [HttpPost]
        [Route("proof")]
        [ProducesResponseType(typeof(IssueProofResult), 200)]
        public async Task<IActionResult> IssueProof([FromServices] HttpIssueProofCommand command, IssueProofRequest request)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (request == null) throw new ArgumentNullException(nameof(request));

            // TODO remove once the custom model binder is written as this will be covered by the validation
            if (!request.UnpackAll(_jsonSerializer))
                return BadRequest("Unable to unpack either commitments or test result");

            if (!ModelState.IsValid)
                return ValidationProblem();

            var result = await command.Execute(request);

            return result;
        }

        [HttpPost]
        [Route("nonce")]
        [ProducesResponseType(typeof(GenerateNonceResult), 200)]
        public async Task<IActionResult> GenerateNonce([FromServices] HttpGenerateNonceCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var result = await command.Execute();

            return result;
        }
    }
}
