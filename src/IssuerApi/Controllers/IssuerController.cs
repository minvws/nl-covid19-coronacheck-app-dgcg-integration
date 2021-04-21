// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Config;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Extensions;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Builders;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Models;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.Attributes;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.ProofOfTest;
using NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerApi.Models;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerApi.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("proof")]
    public class IssuerController : ControllerBase
    {
        private readonly IApiSigningConfig _apiSigningConfig;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ILogger<IssuerController> _logger;
        private readonly IProofOfTestService _potService;
        private readonly IResponseBuilder _srb;

        public IssuerController(
            ILogger<IssuerController> logger,
            IProofOfTestService potService,
            IJsonSerializer jsonSerializer,
            IResponseBuilder responseBuilder,
            IApiSigningConfig apiSigningConfig)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _potService = potService ?? throw new ArgumentNullException(nameof(potService));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            _srb = responseBuilder ?? throw new ArgumentNullException(nameof(responseBuilder));
            _apiSigningConfig = apiSigningConfig ?? throw new ArgumentNullException(nameof(apiSigningConfig));
        }

        /// <summary>
        ///     Issues the proof of (negative) test
        /// </summary>
        [HttpPost]
        [Route("issue")]
        [ProducesResponseType(typeof(IssueProofResult), 200)]
        public IActionResult IssueProof(IssueProofRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"IssueProof: Model validation failed on the fields: {_jsonSerializer.Serialize(ModelState.Keys)}");

                return new BadRequestResult();
            }

            try
            {
                var commitmentsJson = Base64.DecodeAsUtf8String(request.Commitments!);

                var attributes = new ProofOfTestAttributes
                {
                    SampleTime = request.Attributes.SampleTime.ToUnixTime().ToString(),
                    TestType = request.Attributes.TestType,
                    FirstNameInitial = request.Attributes.FirstNameInitial,
                    LastNameInitial = request.Attributes.LastNameInitial,
                    BirthMonth = request.Attributes.BirthMonth,
                    BirthDay = request.Attributes.BirthDay,
                    IsSpecimen = request.Attributes.IsSpecimen ? "1" : "0",
                    IsPaperProof = "0"
                };

                var (proofResult, attributesIssued) = _potService.GetProofOfTest(attributes, request.Nonce!, commitmentsJson, request.Key);

                var issueProofResult = _jsonSerializer.Deserialize<IssueProofResult>(proofResult);
                issueProofResult.AttributesIssued = new IssuerAttributes
                {
                    BirthMonth = attributesIssued.BirthMonth,
                    BirthDay = attributesIssued.BirthDay,
                    FirstNameInitial = attributesIssued.FirstNameInitial,
                    LastNameInitial = attributesIssued.LastNameInitial
                };

                return _apiSigningConfig.WrapAndSignResult
                    ? Ok(_srb.Build(issueProofResult))
                    : Ok(issueProofResult);
            }
            catch (FormatException e)
            {
                _logger.LogError("IssueProof: Error decoding either commitments or issuer message.", e);

                return new BadRequestResult();
            }
            catch (Exception e)
            {
                _logger.LogError("IssueProof: Unexpected exception.", e);

                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        ///     Generates and returns a random nonce
        /// </summary>
        [HttpPost]
        [Route("nonce")]
        [ProducesResponseType(typeof(GenerateNonceResult), 200)]
        public IActionResult GenerateNonce(GenerateNonceRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogDebug("GenerateNonce: Invalid model state.");

                return new BadRequestResult();
            }

            try
            {
                var nonce = _potService.GenerateNonce(request.Key);

                var result = new GenerateNonceResult {Nonce = nonce};

                return _apiSigningConfig.WrapAndSignResult
                    ? Ok(_srb.Build(result))
                    : Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError("IssueProof: Error generating nonce.", e);

                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        ///     Issues a static proof, returning QR in PNG format, base64 encoded in a string
        /// </summary>
        [HttpPost]
        [Route("issue-static")]
        [ProducesResponseType(typeof(IssueStaticProofResult), 200)]
        public IActionResult IssueStaticProof(IssueStaticProofRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"IssueStaticProof: Model validation failed on the fields: {_jsonSerializer.Serialize(ModelState.Keys)}");

                return new BadRequestResult();
            }

            try
            {
                var attributes = new ProofOfTestAttributes
                {
                    SampleTime = request.Attributes.SampleTime.ToUnixTime().ToString(),
                    TestType = request.Attributes.TestType,
                    FirstNameInitial = request.Attributes.FirstNameInitial,
                    LastNameInitial = request.Attributes.LastNameInitial,
                    BirthDay = request.Attributes.BirthDay,
                    BirthMonth = request.Attributes.BirthMonth,
                    IsPaperProof = "1",
                    IsSpecimen = request.Attributes.IsSpecimen ? "1" : "0"
                };

                var (qr, attributesIssued) = _potService.GetStaticProofQr(attributes, request.Key);

                var result = new IssueStaticProofResult
                {
                    Qr = new IssueStaticProofResultQr
                    {
                        Data = qr,
                        AttributesIssued = new IssueStaticProofResultAttributes
                        {
                            //attributesIssued
                            BirthMonth = attributesIssued.BirthMonth,
                            BirthDay = attributesIssued.BirthDay,
                            FirstNameInitial = attributesIssued.FirstNameInitial,
                            LastNameInitial = attributesIssued.LastNameInitial,
                            IsSpecimen = attributesIssued.IsSpecimen,
                            IsPaperProof = "1", // ALWAYS true because paper proof = static proof
                            TestType = attributesIssued.TestType,
                            SampleTime = attributesIssued.SampleTime
                        }
                    }
                };

                return _apiSigningConfig.WrapAndSignResult
                    ? Ok(_srb.Build(result))
                    : Ok(result);
            }
            catch (FormatException e)
            {
                _logger.LogError("IssueStaticProof: Error decoding either commitments or issuer message.", e);

                return new BadRequestResult();
            }
            catch (Exception e)
            {
                _logger.LogError("IssueStaticProof: Error issuing proof.", e);

                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }
    }
}
