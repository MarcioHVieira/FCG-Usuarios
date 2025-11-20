using Fcg.Common.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Prometheus;
using QuestPDF.Infrastructure;
using System.Text.Json.Serialization;
using Usuarios.Api.Infrastructure.Data;

namespace Usuarios.Api.Configurations
{
    public static class ApiConfig
    {
        public static WebApplicationBuilder AddApiConfiguration(this WebApplicationBuilder builder)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            if (builder.Environment.IsDevelopment())
                builder.Configuration.AddUserSecrets<Program>();

            builder.Services.Configure<ApiBehaviorOptions>(options =>
                options.SuppressModelStateInvalidFilter = true);

            builder.Services.AddDbContext<UsuariosDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddApplicationInsightsTelemetry();

            builder.Logging.AddApplicationInsights(
                configureTelemetryConfiguration: (config) =>
                    config.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"],
                configureApplicationInsightsLoggerOptions: (options) => { }
            );

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddAuthorization();
            builder.Services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            return builder;
        }

        public static WebApplication UseApiConfiguration(this WebApplication app)
        {
            app.UseMetricServer();
            app.UseHttpMetrics();
            app.UseMiddleware<TratamentoErrosMiddleware>();
            app.UseAuthorization();
            app.MapControllers();

            return app;
        }
    }
}
