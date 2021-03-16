// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Certificates;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Config;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Signing;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Builders;
using NL.Rijksoverheid.CoronaTester.BackEnd.IssuerInterop;
using System;
using System.Text;
using System.Threading.Tasks;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.IssuerApi
{
    public class Startup
    {
        private IServiceCollection _services;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "IssuerApi", Version = "v1" });
            });

            // Proof of Test API
            services.AddScoped<IUtcDateTimeProvider, StandardUtcDateTimeProvider>();
            services.AddScoped<IProofOfTestService, IssuerProofOfTestService>();
            services.AddScoped<IJsonSerializer, StandardJsonSerializer>();
            services.AddScoped<IIssuerInterop, Issuer>();
            services.AddScoped<ISignedDataResponseBuilder, SignedDataResponseBuilder>();
            services.AddScoped<IContentSigner, CmsSignerSimple>();
            services.AddSingleton<ICertificateLocationConfig, StandardCertificateLocationConfig>();
            services.AddSingleton<IApiSigningConfig, ApiSigningConfig>();

            // Dotnet configuration stuff
            var configuration = ConfigurationRootBuilder.Build();
            services.AddSingleton<IConfiguration>(configuration);

            // Certificate providers
            services.AddSingleton<EmbeddedResourceCertificateProvider, EmbeddedResourceCertificateProvider>();
            services.AddSingleton<FileSystemCertificateProvider, FileSystemCertificateProvider>();
            services.AddSingleton<ICertificateProvider, CertificateProvider>();
            services.AddSingleton<EmbeddedResourcesCertificateChainProvider, EmbeddedResourcesCertificateChainProvider>();
            services.AddSingleton<FileSystemCertificateChainProvider, FileSystemCertificateChainProvider>();
            services.AddSingleton<ICertificateChainProvider, CertificateChainProvider>();

            // Issuer
            services.AddSingleton<IKeyStore, KeyStore>();
            services.AddSingleton<AssemblyKeyStore, AssemblyKeyStore>();
            services.AddSingleton<FileSystemKeyStore, FileSystemKeyStore>();
            services.AddSingleton<IIssuerCertificateConfig, IssuerCertificateConfig>();

            _services = services;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (env == null) throw new ArgumentNullException(nameof(env));
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            LogInitializationBanner(env, logger);

            logger.LogInformation("Enabling support for reverse proxy headers");
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (env.IsDevelopment())
            {
                logger.LogWarning("Development Mode is active!");

                logger.LogInformation("Development Mode: Exceptions will be displayed.");
                app.UseDeveloperExceptionPage();

                logger.LogInformation("Development Mode: Swagger interface activate.");
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProofOfTestAPI v1"));

            }
            else
            {
                logger.LogInformation("Production Mode is active!");

                logger.LogInformation("Supressing exception message body");
                app.UseExceptionHandler(a => a.Run(context => Task.CompletedTask));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private void LogInitializationBanner(IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env == null) throw new ArgumentNullException(nameof(env));
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            var message = new StringBuilder();

            logger.LogInformation("Initializing ProofOfTestAPI");

            message.AppendLine("Service Registration information");
            message.AppendLine($"The following {_services.Count} services have been registered.");
            foreach (var service in _services)
            {
                message.AppendLine(
                    $"{service.ServiceType.Name} > {service.ImplementationType?.Name} [{service.Lifetime}]");
            }
            logger.LogInformation(message.ToString());

            message.Clear();
            message.AppendLine("Runtime Environment information");
            message.AppendLine($"Application name: {env.ApplicationName}");
            message.AppendLine($"Path: {env.ContentRootPath}");
            message.AppendLine($"Environment name: {env.EnvironmentName}");

            logger.LogInformation(message.ToString());
        }
    }
}
