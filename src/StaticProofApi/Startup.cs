// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web;
using NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerApi.Client;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.StaticProofApi
{
    public class Startup : StartupCommonBase<Startup>
    {
        protected override string ServiceName => "StaticProofApi";

        public override void ConfigureServices(IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddTestProviderSignatureValidation();
            services.AddIssuerApiClient();
            services.AddTestResultLog();
            services.AddHttpClient();
            services.AddApiVersioning();

            // Register an HTTP client configuration which allows self-signed SSL certificates
            services.AddHttpClient("AllowSelfSignedCertificatesHandler").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
            });

            base.ConfigureServices(services);
        }
    }
}
