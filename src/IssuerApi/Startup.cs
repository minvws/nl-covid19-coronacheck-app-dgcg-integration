// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Config;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web;
using NL.Rijksoverheid.CoronaTester.BackEnd.IssuerInterop;
using System;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.IssuerApi
{
    public class Startup : StartupCommonBase<Startup>
    {
        protected override string ServiceName => "IssuerApi";

        public override void ConfigureServices(IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            // Dotnet configuration stuff
            var configuration = ConfigurationRootBuilder.Build();
            services.AddSingleton<IConfiguration>(configuration);
            
            // Issuer
            services.AddProofOfTestService();
            services.AddScoped<IIssuerInterop, Issuer>();
            
            base.ConfigureServices(services);
        }
    }
}
