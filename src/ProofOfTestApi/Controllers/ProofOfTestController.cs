// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.AspNetCore.Mvc;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Config;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Extensions;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Builders;
using NL.Rijksoverheid.CoronaTester.BackEnd.IssuerInterop;
using NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Models;
using System;
using System.Net;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Controllers
{
    [ApiController]
    [Route("proof")]
    public class ProofOfTestController : ControllerBase
    {
        private readonly IProofOfTestService _potService;
        private readonly IUtcDateTimeProvider _dateTimeProvider;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ISignedDataResponseBuilder _srb;
        private readonly IApiSigningConfig _apiSigningConfig;

        public ProofOfTestController(
            IProofOfTestService potService,
            IUtcDateTimeProvider dateTimeProvider,
            IJsonSerializer jsonSerializer,
            ISignedDataResponseBuilder signedDataResponseBuilder,
            IApiSigningConfig apiSigningConfig)
        {
            _potService = potService ?? throw new ArgumentNullException(nameof(potService));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            _srb = signedDataResponseBuilder ?? throw new ArgumentNullException(nameof(signedDataResponseBuilder));
            _apiSigningConfig = apiSigningConfig ?? throw new ArgumentNullException(nameof(apiSigningConfig));
        }

        /// <summary>
        /// Issues the proof of (negative) test
        /// </summary>
        [HttpPost]
        [Route("issue")]
        [ProducesResponseType(typeof(IssueProofResult), 200)]
        public IActionResult IssueProof(IssueProofRequest request)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestResult();
            }

            if (request == null)
                return new BadRequestResult();

            try
            {
                var commitmentsJson = Base64.Decode(request.Commitments);
                var attributes = new ProofOfTestAttributes(request.SampleTime, request.TestType);

                var proofResult =
                    _potService.GetProofOfTest(attributes, request.Nonce, commitmentsJson);

                var issuerMessage = _jsonSerializer.Deserialize<IssueSignatureMessage>(proofResult);

                var issueProofResult = new IssueProofResult
                {
                    Ism = issuerMessage,
                    Attributes = new Attributes
                    {
                        SampleTime = attributes.SampleTime,
                        TestType = attributes.TestType
                    },
                    SessionToken = request.SessionToken
                };

                return _apiSigningConfig.WrapAndSignResult
                    ? Ok(_srb.Build(issueProofResult))
                    : Ok(issueProofResult);
            }
            catch (IssuerException)
            {
                // todo IssuerException as urgent/actionable
                // todo JsonDeserialized exception as urgent/actionable

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            catch (FormatException)
            {
                return new BadRequestResult();
            }
        }

        /// <summary>
        /// Generates and returns a random nonce
        /// </summary>
        [HttpPost]
        [Route("nonce")]
        [ProducesResponseType(typeof(GenerateNonceResult), 200)]
        public IActionResult GenerateNonce(GenerateNonceRequest request)
        {
            if (request == null)
                return new BadRequestResult();

            var nonce = _potService.GenerateNonce();

            var result = new GenerateNonceResult {Nonce = nonce, SessionToken = request.SessionToken};
            
            return _apiSigningConfig.WrapAndSignResult
                ? Ok(_srb.Build(result))
                : Ok(result);
        }
    }
}
