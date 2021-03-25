// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.Extensions.DependencyInjection;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web;
using NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerApi.Client;
using System;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.ProofOfTestApi
{
    public class Startup : StartupCommonBase<Startup>
    {
        protected override string ServiceName => "ProofOfTestApi";

        public override void ConfigureServices(IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddHttpClient();
            services.AddTestResultLog();
            services.AddTestProviderSignatureValidation();
            services.AddIssuerApiClient();
            services.AddRedisSessionStore();

            base.ConfigureServices(services);
        }
    }
}
