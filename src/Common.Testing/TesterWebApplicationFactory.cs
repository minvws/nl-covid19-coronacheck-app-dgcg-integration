// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Models;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Testing
{
    public abstract class TesterWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        private readonly StandardJsonSerializer _jsonSerializer;
        protected readonly WebApplicationFactory<TEntryPoint> Factory;

        protected TesterWebApplicationFactory()
        {
            Factory = WithWebHostBuilder(builder => { builder.ConfigureTestServices(services => { }); });
            _jsonSerializer = new StandardJsonSerializer();
        }

        protected T Unwrap<T>(string content) where T : class
        {
            if (string.IsNullOrWhiteSpace(content)) throw new ArgumentNullException(nameof(content));

            if (!content.Contains("payload")) return _jsonSerializer.Deserialize<T>(content);

            var signingWrapperResult = _jsonSerializer.Deserialize<SignedDataWrapper<T>>(content);

            if (signingWrapperResult.Payload == null || signingWrapperResult.Signature == null) throw new Exception("Payload or signature missing");

            return _jsonSerializer.Deserialize<T>(Base64.DecodeAsUtf8String(signingWrapperResult.Payload));
        }
    }
}
