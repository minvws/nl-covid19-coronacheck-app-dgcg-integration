// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.Extensions.DependencyInjection;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerApi.Client
{
    public static class ContainerMappingExtensions
    {
        public static void AddIssuerApiClient(this IServiceCollection services)
        {
            services.AddScoped<IIssuerApiClient, IssuerApiClient>();
            services.AddSingleton<IIssuerApiClientConfig, IssuerApiClientConfig>();
        }
    }
}
