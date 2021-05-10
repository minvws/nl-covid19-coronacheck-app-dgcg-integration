﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Certificates;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Config;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Signing;
using NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Client;
using Microsoft.AspNetCore.Authentication.Certificate;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal class Program
    {
        private static void Main(string[] args)
        {
            var services = new ServiceCollection();

            Parser.Default.ParseArguments<Options>(args)
                  .WithParsed(opt => ConfigureContainer(services, opt))
                  .WithNotParsed(HandleParseError);

            using var serviceProvider = services.BuildServiceProvider();
            // PrintConfig(serviceProvider);
            var app = serviceProvider.GetRequiredService<DgcgApp>();
            app.Run().Wait();
        }

        private static void ConfigureContainer(ServiceCollection services, Options options)
        {
            services.AddSingleton(options);
            services.AddSingleton<IDgcgClientConfig, DgcgClientConfig>();
            services.AddSingleton<ICertificateLocationConfig, StandardCertificateLocationConfig>();
            services.AddTransient<HttpClient>();
            services.AddTransient<IDgcgClient, DgcgClient>();
            services.AddTransient<IUtcDateTimeProvider, StandardUtcDateTimeProvider>();
            services.AddTransient<IJsonSerializer, StandardJsonSerializer>();
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
                        new StandardCertificateLocationConfig(x.GetRequiredService<IConfiguration>(), "Certificates:Signing"),
                        x.GetRequiredService<ILogger<CertificateProvider>>()
                    ),
                    new CertificateChainProvider(
                        new StandardCertificateLocationConfig(x.GetRequiredService<IConfiguration>(), "Certificates:SigningChain"),
                        x.GetRequiredService<ILogger<CertificateChainProvider>>()
                    ),
                    x.GetRequiredService<IUtcDateTimeProvider>()
                ));

            // Defaults for client authentication
            services
               .AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
               .AddCertificate();

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

        // ReSharper disable once UnusedMember.Local
        private static void PrintConfig(IServiceProvider serviceProvider)
        {
            var serializer = serviceProvider.GetRequiredService<IJsonSerializer>();

            Console.WriteLine("** START CONFIGURATION **");

            var conf1 = serviceProvider.GetRequiredService<IDgcgClientConfig>();
            Console.WriteLine("DgcgClient");
            Console.WriteLine(serializer.Serialize(conf1));

            var conf2 = new StandardCertificateLocationConfig(serviceProvider.GetRequiredService<IConfiguration>(), "Certificates:Signing");
            Console.WriteLine("Certificates:Signing");
            Console.WriteLine(serializer.Serialize(conf2));

            var conf3 = new StandardCertificateLocationConfig(serviceProvider.GetRequiredService<IConfiguration>(), "Certificates:SigningChain");
            Console.WriteLine("Certificates:SigningChain");
            Console.WriteLine(serializer.Serialize(conf3));

            var conf4 = new StandardCertificateLocationConfig(serviceProvider.GetRequiredService<IConfiguration>(), "Certificates:Authentication");
            Console.WriteLine("Certificates:Authentication");
            Console.WriteLine(serializer.Serialize(conf4));

            Console.WriteLine("** END CONFIGURATION **");
        }
    }
}
