// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Certificates;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Config;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Files;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Signing;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Builders;
using NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Config;
using NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Services;
using System;
using Microsoft.Extensions.Logging;

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
            // Configuration
            // services.AddSingleton<IRedisSessionCacheConfig, RedisSessionCacheConfig>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "ProofOfTestApi", Version = "v1"});
            });

            // var sessionCacheConfig = ResolveSessionCacheConfig(services);

            //services.AddStackExchangeRedisCache(action =>
            //{
            //    action.InstanceName = sessionCacheConfig.InstanceName;
            //    action.Configuration = sessionCacheConfig.Configuration;
            //});

            //services.AddSession(options =>
            //{
            //    options.IdleTimeout = TimeSpan.FromSeconds(StaticConfig.SessionTimeout);
            //    options.Cookie.HttpOnly = true;
            //    options.Cookie.IsEssential = true;
            //});

            services.AddHttpClient();

            // API specific
            services.AddScoped<ITestResultLog, RedisTestResultLog>();
            services.AddScoped<IRedisTestResultLogConfig, RedisTestResultLogConfig>();
            services.AddScoped<IIssuerApiClient, IssuerApiClient>();
            services.AddScoped<ISessionDataStore, RedisSessionStore>();
            services.AddSingleton<IRedisSessionStoreConfig, RedisSessionStoreConfig>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IIssuerApiClientConfig, IssuerApiClientConfig>();

            // Test validation (one shared instance)
            services.AddSingleton<IFileLoader, FileSystemFileLoader>();
            services.AddSingleton<ITestProviderSignatureValidatorConfig, TestProviderSignatureValidatorConfig>();
            //services.AddSingleton<ITestProviderSignatureValidator, TestProviderSignatureValidator>();
            services.AddSingleton<ITestProviderSignatureValidator>(provider =>
            {
                var config = provider.GetService<ITestProviderSignatureValidatorConfig>();
                var fileLoader = provider.GetService<IFileLoader>();
                var log = provider.GetService<ILogger<TestProviderSignatureValidator>>();
                var instance = new TestProviderSignatureValidator(config, fileLoader, log);
                instance.Initialize();
                return instance;
            });
            
            // Commonly used services
            services.AddScoped<IJsonSerializer, StandardJsonSerializer>();
            services.AddScoped<IUtcDateTimeProvider, StandardUtcDateTimeProvider>();

            // Response signer
            services.AddScoped<ISignedDataResponseBuilder, SignedDataResponseBuilder>();
            services.AddScoped<IContentSigner, CmsSignerSimple>();
            services.AddSingleton<ICertificateLocationConfig, StandardCertificateLocationConfig>();
            services.AddSingleton<IApiSigningConfig, ApiSigningConfig>();

            // Certificate providers
            services.AddSingleton<EmbeddedResourceCertificateProvider, EmbeddedResourceCertificateProvider>();
            services.AddSingleton<FileSystemCertificateProvider, FileSystemCertificateProvider>();
            services.AddSingleton<ICertificateProvider, CertificateProvider>();
            services.AddSingleton<EmbeddedResourcesCertificateChainProvider, EmbeddedResourcesCertificateChainProvider>();
            services.AddSingleton<FileSystemCertificateChainProvider, FileSystemCertificateChainProvider>();
            services.AddSingleton<ICertificateChainProvider, CertificateChainProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProofOfTestApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthorization();

            // NOTE: this must be after .UseRouting() but before .UseEndpoints()
            //app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });



        }

        //private static IRedisSessionCacheConfig ResolveSessionCacheConfig(IServiceCollection services)
        //{
        //    var provider = services.BuildServiceProvider();
        //    return provider.GetService<IRedisSessionCacheConfig>() ?? throw new InvalidOperationException("No IRedisSessionCacheConfig registered ");
        //}
    }
}
