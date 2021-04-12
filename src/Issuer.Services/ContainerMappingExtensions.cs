// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using Microsoft.Extensions.DependencyInjection;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.Keystores;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.ProofOfTest;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services
{
    public static class ContainerMappingExtensions
    {
        public static void AddProofOfTestService(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<IProofOfTestService, ProofOfTestService>();
            services.AddSingleton<IKeyStore, KeyStore>();
            services.AddSingleton<AssemblyKeyStore, AssemblyKeyStore>();
            services.AddSingleton<FileSystemKeyStore, FileSystemKeyStore>();
            services.AddSingleton<IKeyStoreConfig, KeyStoreConfig>();
            services.AddSingleton<IProofOfTestServiceConfig, ProofOfTestServiceConfig>();
        }
    }
}
