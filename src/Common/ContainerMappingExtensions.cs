// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.Extensions.DependencyInjection;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Certificates;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Config;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common
{
    public static class ContainerMappingExtensions
    {
        internal static void AddCertificateProvider(this IServiceCollection services)
        {

            services.AddSingleton<EmbeddedResourceCertificateProvider, EmbeddedResourceCertificateProvider>();
            services.AddSingleton<FileSystemCertificateProvider, FileSystemCertificateProvider>();
            services.AddSingleton<ICertificateProvider, CertificateProvider>();
        }

        internal static void AddCertificateChainProvider(this IServiceCollection services)
        {
            services.AddSingleton<EmbeddedResourcesCertificateChainProvider, EmbeddedResourcesCertificateChainProvider>();
            services.AddSingleton<FileSystemCertificateChainProvider, FileSystemCertificateChainProvider>();
            services.AddSingleton<ICertificateChainProvider, CertificateChainProvider>();
        }

        public static void AddCertificateProviders(this IServiceCollection services)
        {
            services.AddCertificateChainProvider();
            services.AddCertificateProvider();
        }

        /// <summary>
        /// Registers super-common single implementation services such as the Json serializers and UTC datetime provider
        /// </summary>
        public static void AddCommon(this IServiceCollection services)
        {
            services.AddSingleton<IJsonSerializer, StandardJsonSerializer>();
            services.AddSingleton<IUtcDateTimeProvider, StandardUtcDateTimeProvider>();
        }

        public static void AddProofOfTestService(this IServiceCollection services)
        {
            services.AddScoped<IProofOfTestService, IssuerProofOfTestService>();
            services.AddSingleton<IKeyStore, KeyStore>();
            services.AddSingleton<AssemblyKeyStore, AssemblyKeyStore>();
            services.AddSingleton<FileSystemKeyStore, FileSystemKeyStore>();
            services.AddSingleton<IIssuerCertificateConfig, IssuerCertificateConfig>();
            services.AddSingleton<IIssuerConfig, IssuerConfig>();
        }
    }
}
