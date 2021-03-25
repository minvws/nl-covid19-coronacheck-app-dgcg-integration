// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.AspNetCore.Mvc;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Config;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Controllers
{
    public abstract class MiddlewareControllerBase : ControllerBase 
    {
        protected readonly IJsonSerializer JsonSerializer;
        protected readonly IUtcDateTimeProvider UtcDateTimeProvider;
        protected readonly IApiSigningConfig ApiSigningConfig;
        private readonly ISignedDataResponseBuilder _srb;

        protected MiddlewareControllerBase(IJsonSerializer jsonSerializer, IUtcDateTimeProvider utcDateTimeProvider, ISignedDataResponseBuilder signedDataResponseBuilder, IApiSigningConfig apiSigningConfig)
        {
            JsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            UtcDateTimeProvider = utcDateTimeProvider ?? throw new ArgumentNullException(nameof(utcDateTimeProvider));
            ApiSigningConfig = apiSigningConfig ?? throw new ArgumentNullException(nameof(apiSigningConfig));
            _srb = signedDataResponseBuilder ?? throw new ArgumentNullException(nameof(signedDataResponseBuilder));
        }
        
        protected OkObjectResult OkWrapped<T>(T result)
        {
            return Ok(_srb.Build(result));
        }

        protected static bool IsValid<T>(T req)
        {
            var context = new ValidationContext(req);
            var results = new List<ValidationResult>();

            return Validator.TryValidateObject(req, context, results);
        }
    }
}