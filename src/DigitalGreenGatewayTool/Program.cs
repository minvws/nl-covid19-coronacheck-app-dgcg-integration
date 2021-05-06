using System;
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
using NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Client;

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
                    x.GetRequiredService<ICertificateLocationConfig>(),
                    x.GetRequiredService<EmbeddedResourceCertificateProvider>(),
                    x.GetRequiredService<FileSystemCertificateProvider>(),
                    x.GetRequiredService<ILogger<CertificateProvider>>()
                ));

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
