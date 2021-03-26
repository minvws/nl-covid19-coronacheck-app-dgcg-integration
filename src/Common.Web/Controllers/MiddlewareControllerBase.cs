// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Config;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Builders;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Controllers
{
    public abstract class MiddlewareControllerBase : ControllerBase
    {
        private readonly ISignedDataResponseBuilder _srb;
        protected readonly IApiSigningConfig ApiSigningConfig;
        protected readonly IJsonSerializer JsonSerializer;
        protected readonly IUtcDateTimeProvider UtcDateTimeProvider;

        protected MiddlewareControllerBase(IJsonSerializer jsonSerializer, IUtcDateTimeProvider utcDateTimeProvider,
                                           ISignedDataResponseBuilder signedDataResponseBuilder, IApiSigningConfig apiSigningConfig)
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
            if (req == null) throw new ArgumentNullException(nameof(req));

            var context = new ValidationContext(req);
            var results = new List<ValidationResult>();

            return Validator.TryValidateObject(req, context, results);
        }
    }
}
