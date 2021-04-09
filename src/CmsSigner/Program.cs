// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using CmsSigner.Certificates;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Certificates;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Signing;

namespace CmsSigner
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
            var app = serviceProvider.GetRequiredService<CmsSignerApp>();
            var options = serviceProvider.GetRequiredService<Options>();
            var inputFile = Load(options.InputFile!);
            app.Run(inputFile, options.Validate);
        }

        private static void ConfigureContainer(ServiceCollection services, Options options)
        {
            // This data must be loaded now as it's used by the classes injected into the DI container
            var certFile = Load(options.SigningCertificateFile!);
            var chainFile = Load(options.CertificateChainFile!);

            services.AddLogging();
            services.AddTransient<ICmsValidator, CmsValidator>();
            services.AddTransient<IContentSigner, NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Signing.CmsSigner>();
            services.AddTransient<IUtcDateTimeProvider, StandardUtcDateTimeProvider>();
            services.AddTransient<IJsonSerializer, StandardJsonSerializer>();
            services.AddTransient<ICertificateProvider, CertProvider>(provider => new CertProvider(CertificateHelpers.Load(certFile, options.Password!)));
            services.AddTransient<ICertificateChainProvider, ChainProvider>(provider => new ChainProvider(CertificateHelpers.LoadAll(chainFile)));
            services.AddTransient(provider => options);
            services.AddTransient<CmsSignerApp>();
        }

        private static byte[] Load(string path)
        {
            try
            {
                return File.ReadAllBytes(path);
            }
            catch
            {
                // ignored
            }

            Console.WriteLine($"ERROR: unable to open file: {path}");

            Environment.Exit(1);

            return null;
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            Console.WriteLine("Error parsing input, please check your call and try again.");

            Environment.Exit(0);
        }
    }
}
