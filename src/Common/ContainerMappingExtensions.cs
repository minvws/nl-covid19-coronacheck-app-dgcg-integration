// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using Microsoft.Extensions.DependencyInjection;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Certificates;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common
{
    public static class ContainerMappingExtensions
    {
        private static void AddCertificateProvider(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<EmbeddedResourceCertificateProvider, EmbeddedResourceCertificateProvider>();
            services.AddSingleton<FileSystemCertificateProvider, FileSystemCertificateProvider>();
            services.AddSingleton<ICertificateProvider, CertificateProvider>();
        }

        private static void AddCertificateChainProvider(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<EmbeddedResourcesCertificateChainProvider, EmbeddedResourcesCertificateChainProvider>();
            services.AddSingleton<FileSystemCertificateChainProvider, FileSystemCertificateChainProvider>();
            services.AddSingleton<ICertificateChainProvider, CertificateChainProvider>();
        }

        public static void AddCertificateProviders(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddCertificateChainProvider();
            services.AddCertificateProvider();
        }

        /// <summary>
        ///     Registers super-common single implementation services such as the Json serializers and UTC datetime provider
        /// </summary>
        public static void AddCommon(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IJsonSerializer, StandardJsonSerializer>();
            services.AddSingleton<IUtcDateTimeProvider, StandardUtcDateTimeProvider>();
        }
    }
}
