// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

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

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProofOfTestAPI", Version = "v1" });
            });


            // Proof of Test API
            services.AddScoped<IUtcDateTimeProvider, StandardUtcDateTimeProvider>();
            services.AddScoped<IKeyStore, AssemblyKeyStore>();
            services.AddScoped<IProofOfTestService, IssuerProofOfTestService>();
            services.AddScoped<IJsonSerializer, StandardJsonSerializer>();
            services.AddScoped<IIssuerInterop, Issuer>();
            services.AddScoped<ISignedDataResponseBuilder, SignedDataResponseBuilder>();
            services.AddScoped<ZippedSignedContentFormatter>();
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Support for a reverse proxy
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable swagger everywhere temporarily!
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProofOfTestAPI v1"));

            app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
