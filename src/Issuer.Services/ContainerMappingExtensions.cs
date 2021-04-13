// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using Microsoft.Extensions.DependencyInjection;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.Keystores;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.PartialDisclosure;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.ProofOfTest;
using NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerInterop;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services
{
    public static class ContainerMappingExtensions
    {
        public static void AddProofOfTestService(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<IProofOfTestService, ProofOfTestService>();
            services.AddScoped<IIssuerInterop, IssuerInterop.Issuer>();
            services.AddSingleton<IKeyStore, KeyStore>();
            services.AddSingleton<AssemblyKeyStore, AssemblyKeyStore>();
            services.AddSingleton<FileSystemKeyStore, FileSystemKeyStore>();
            services.AddSingleton<IKeyStoreConfig, KeyStoreConfig>();
            services.AddSingleton<IProofOfTestServiceConfig, ProofOfTestServiceConfig>();
        }

        public static void AddPartialDisclosure(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IPartialDisclosureService, PartialDisclosureService>();
            services.AddSingleton<IPartialDisclosureListProvider, PartialDisclosureListProvider>();
            services.AddSingleton<IPartialDisclosureServiceConfig, PartialDisclosureServiceConfig>();
        }
    }
}
