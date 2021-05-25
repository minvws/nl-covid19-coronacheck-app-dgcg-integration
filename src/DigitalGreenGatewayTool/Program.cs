// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using CommandLine;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Certificates;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Config;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Signing;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Builders;
using NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Client;
using NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Formatters;
using NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Validator;
using NLog;
using NLog.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var services = new ServiceCollection();

                Parser.Default.ParseArguments<Options>(args)
                      .WithParsed(opt => ConfigureContainer(services, opt))
                      .WithNotParsed(HandleParseError);

                using var serviceProvider = services.BuildServiceProvider();
                var options = serviceProvider.GetRequiredService<Options>();
                options.ValidateSelectedOptions();
                var app = serviceProvider.GetRequiredService<DgcgApp>();
                app.Run().Wait();

                if (options.Pause)
                {
                    Console.WriteLine();
                    Console.WriteLine("Finished! Press ENTER to exit.");
                    Console.ReadLine();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error with NLog configuration");

                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
            }
        }

        private static void ConfigureContainer(ServiceCollection services, Options options)
        {
            services.AddLogging(loggingBuilder =>
            {
                // configure Logging with NLog
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                loggingBuilder.AddNLog("nlog.config");
            });

            services.AddSingleton(options);
            services.AddSingleton<IDgcgClientConfig, DgcgClientConfig>();
            services.AddSingleton<ICertificateLocationConfig, StandardCertificateLocationConfig>();
            services.AddTransient<HttpClient>();
            services.AddTransient<IDgcgClient, DgcgClient>();
            services.AddTransient<IUtcDateTimeProvider, StandardUtcDateTimeProvider>();
            services.AddTransient<IJsonSerializer, UnsafeJsonSerializer>();
            services.AddCertificateProviders();
            services.AddLogging();

            services.AddSingleton<ICertificateLocationConfig>(
                x => new StandardCertificateLocationConfig(x.GetRequiredService<IConfiguration>(), "Certificates:Authentication"));

            services.AddTransient<IAuthenticationCertificateProvider>(
                x => new CertificateProvider(
                    new StandardCertificateLocationConfig(x.GetRequiredService<IConfiguration>(), "Certificates:Authentication"),
                    x.GetRequiredService<ILogger<CertificateProvider>>()
                ));

            services.AddTransient<IContentSigner>(
                x => new CmsSigner(
                    new CertificateProvider(
                        new StandardCertificateLocationConfig(x.GetRequiredService<IConfiguration>(), "Certificates:UploadSignature"),
                        x.GetRequiredService<ILogger<CertificateProvider>>()
                    ),
                    new CertificateChainProvider(
                        new StandardCertificateLocationConfig(x.GetRequiredService<IConfiguration>(), "Certificates:UploadSignatureChain"),
                        x.GetRequiredService<ILogger<CertificateChainProvider>>()
                    ),
                    x.GetRequiredService<IUtcDateTimeProvider>()
                ));

            services.AddTransient(
                x => new TrustListValidator(
                    new CertificateProvider(
                        new StandardCertificateLocationConfig(x.GetRequiredService<IConfiguration>(), "Certificates:TrustAnchor"),
                        x.GetRequiredService<ILogger<CertificateProvider>>()
                    )
                ));

            // Defaults for client authentication
            services
               .AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
               .AddCertificate();

            // Register formatter
            if (options.Unformatted)
                services.AddTransient<ITrustListFormatter, DgcgJsonFormatter>();
            else
                services.AddTransient<ITrustListFormatter, DutchFormatter>();

            // Register the response wrapping
            if (options.Wrap)
                services.AddTransient<IResponseBuilder>(
                    serviceProvider =>
                    {
                        var serializer = serviceProvider.GetRequiredService<IJsonSerializer>();
                        var signer = new CmsSigner(
                            new CertificateProvider(
                                new StandardCertificateLocationConfig(serviceProvider.GetRequiredService<IConfiguration>(), "Certificates:CmsSignature"),
                                serviceProvider.GetRequiredService<ILogger<CertificateProvider>>()
                            ),
                            new CertificateChainProvider(
                                new StandardCertificateLocationConfig(serviceProvider.GetRequiredService<IConfiguration>(), "Certificates:CmsSignatureChain"),
                                serviceProvider.GetRequiredService<ILogger<CertificateChainProvider>>()
                            ),
                            serviceProvider.GetRequiredService<IUtcDateTimeProvider>()
                        );

                        return new SignedResponseBuilder(serializer, signer);
                    });
            else
                services.AddTransient<IResponseBuilder, StandardResponseBuilder>();

            // Dotnet configuration stuff
            var configuration = ConfigurationRootBuilder.Build();
            services.AddSingleton<IConfiguration>(configuration);

            services.AddTransient(provider => options);
            services.AddTransient<DgcgApp>();
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            Console.WriteLine("Error parsing input, please check your call and try again.");

            Environment.Exit(0);
        }
    }
}
