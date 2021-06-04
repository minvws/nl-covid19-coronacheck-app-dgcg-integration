// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using Microsoft.Extensions.DependencyInjection;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Config;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Signing;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Builders;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web
{
    public static class ContainerMappingExtensions
    {
        public static void AddResponseSigner(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<IContentSigner, CmsSigner>();
            services.AddSingleton<ICertificateLocationConfig, StandardCertificateLocationConfig>();
            services.AddSingleton<IApiSigningConfig, ApiSigningConfig>();

            //
            // The configuration decides how responses are packaged
            //

            var serviceProvider = services.BuildServiceProvider();
            var signingConfig = serviceProvider.GetRequiredService<IApiSigningConfig>();
            if (signingConfig.WrapAndSignResult)
                services.AddSingleton<IResponseBuilder, SignedResponseBuilder>();
            else
                services.AddSingleton<IResponseBuilder, StandardResponseBuilder>();
        }
    }
}
