// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Config;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Files;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Signing;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Builders;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Commands;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Config;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Signatures;

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
                services.AddSingleton<IResponseBuilder, ResponseBuilder>();
            else
                services.AddSingleton<IResponseBuilder, StandardResponseBuilder>();

            //// Decide via the factory call
            //services.AddScoped<IResponseBuilder>(sp =>
            //{
            //    var config = sp.GetRequiredService<IApiSigningConfig>();
            //    if (!config.WrapAndSignResult) return new StandardResponseBuilder();
            //    var serializer = sp.GetRequiredService<IJsonSerializer>();
            //    var signer = sp.GetRequiredService<IContentSigner>();

            //    return new ResponseBuilder(serializer, signer);
            //});
        }

        public static void AddTestProviderSignatureValidation(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IFileLoader, FileSystemFileLoader>();
            services.AddSingleton<ITestProviderSignatureValidatorConfig, TestProviderSignatureValidatorConfig>();
            services.AddSingleton(provider =>
            {
                var config = provider.GetService<ITestProviderSignatureValidatorConfig>();
                var fileLoader = provider.GetService<IFileLoader>();
                var log = provider.GetService<ILogger<TestProviderSignatureValidator>>();
                ITestProviderSignatureValidator instance = new TestProviderSignatureValidator(config, fileLoader, log!);
                instance.Initialize();
                return instance;
            });
        }

        public static void AddTestResultLog(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<ITestResultLog, RedisTestResultLog>();
            services.AddScoped<IRedisTestResultLogConfig, RedisTestResultLogConfig>();
        }

        public static void AddRedisSessionStore(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<ISessionDataStore, RedisSessionStore>();
            services.AddSingleton<IRedisSessionStoreConfig, RedisSessionStoreConfig>();
        }
    }
}
