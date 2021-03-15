// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Models;
using System;
using Xunit;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Testing
{
    public abstract class TesterWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        protected readonly WebApplicationFactory<TEntryPoint> Factory;
        protected readonly StandardJsonSerializer JsonSerializer;

        protected TesterWebApplicationFactory()
        {
            Factory = WithWebHostBuilder(builder => { builder.ConfigureTestServices(services => { }); });
            JsonSerializer = new StandardJsonSerializer();
        }

        protected T Unwrap<T>(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) throw new ArgumentNullException(nameof(content));

            if (content.Contains("payload"))
            {
                var signingWrapperResult = JsonSerializer.Deserialize<SignedDataResponse<T>>(content);
                Assert.NotEmpty(signingWrapperResult.Payload);
                Assert.NotEmpty(signingWrapperResult.Signature);
                var payloadString = Base64.Decode(signingWrapperResult.Payload);
                return JsonSerializer.Deserialize<T>(payloadString);
            }

            return JsonSerializer.Deserialize<T>(content);
        }
    }
}