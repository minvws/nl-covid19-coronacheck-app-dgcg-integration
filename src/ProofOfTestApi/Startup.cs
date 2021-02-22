// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Certificates;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Config;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Signing;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Builders;
using NL.Rijksoverheid.CoronaTester.BackEnd.IssuerInterop;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi
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
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "ProofOfTestAPI", Version = "v1"});
            });

            // Proof of Test API
            services.AddScoped<IUtcDateTimeProvider, StandardUtcDateTimeProvider>();
            services.AddScoped<IProofOfTestService, IssuerProofOfTestService>();
            services.AddScoped<IJsonSerializer, StandardJsonSerializer>();
            services.AddScoped<IIssuerInterop, Issuer>();
            services.AddScoped<ISignedDataResponseBuilder, SignedDataResponseBuilder>();
            services.AddScoped<IContentSigner, CmsSignerSimple>();
            services.AddScoped<ICertificateLocationConfig, StandardCertificateLocationConfig>();
            services.AddSingleton<IApiSigningConfig, ApiSigningConfig>();

            // Dotnet configuration stuff
            var configuration = ConfigurationRootBuilder.Build();
            services.AddSingleton<IConfiguration>(configuration);

            // Certificate providers
            services.AddScoped<EmbeddedResourceCertificateProvider, EmbeddedResourceCertificateProvider>();
            services.AddScoped<FileSystemCertificateProvider, FileSystemCertificateProvider>();
            services.AddScoped<ICertificateProvider, CertificateProvider>();
            services.AddScoped<EmbeddedResourcesCertificateChainProvider, EmbeddedResourcesCertificateChainProvider>();
            services.AddScoped<FileSystemCertificateChainProvider, FileSystemCertificateChainProvider>();
            services.AddScoped<ICertificateChainProvider, CertificateChainProvider>();

            // Issuer
            services.AddScoped<IKeyStore, KeyStore>();
            services.AddScoped<AssemblyKeyStore, AssemblyKeyStore>();
            services.AddScoped<FileSystemKeyStore, FileSystemKeyStore>();
            services.AddScoped<IIssuerCertificateConfig, IssuerCertificateConfig>();

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
            
            logger.LogInformation($"Initializing ProofOfTestAPI");
            logger.LogInformation($"Service Registration information");
            logger.LogInformation($"The following {_services.Count} services have been registered.");
            foreach (var service in _services)
            {
                logger.LogInformation(
                    $"{service.ServiceType.Name} > {service.ImplementationType?.Name} [{service.Lifetime}]");
            }

            logger.LogInformation($"Runtime Environment information");
            logger.LogInformation($"Application name: {env.ApplicationName}");
            logger.LogInformation($"Path: {env.ContentRootPath}");
            logger.LogInformation($"Environment name: {env.EnvironmentName}");
        }
    }
}
