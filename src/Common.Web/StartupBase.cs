// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web
{
    public abstract class StartupCommonBase<T>
    {
        private IServiceCollection _services;

        protected abstract string ServiceName { get; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = ServiceName, Version = "v1" });
            });

            services.AddControllers();
            services.AddCommon();
            services.AddResponseSigner();
            services.AddCertificateProviders();

            _services = services;
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<T> logger)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (env == null) throw new ArgumentNullException(nameof(env));
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            LogInitializationBanner(env, logger);

            if (env.IsDevelopment())
            {
                logger.LogWarning("Development Mode is active!");

                logger.LogInformation("Development Mode: Exceptions will be displayed.");
                app.UseDeveloperExceptionPage();

                logger.LogInformation("Development Mode: Swagger interface activate.");
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", ServiceName));
            }
            else
            {
                logger.LogInformation("Production Mode is active!");

                logger.LogInformation("Suppressing exception message body");
                app.UseExceptionHandler(a => a.Run(context => Task.CompletedTask));
            }

            logger.LogInformation("Enabling support for reverse proxy headers");
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        
        private void LogInitializationBanner(IWebHostEnvironment env, ILogger<T> logger)
        {
            if (env == null) throw new ArgumentNullException(nameof(env));
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            var message = new StringBuilder();

            logger.LogInformation($"Initializing ${nameof(T)}");

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
