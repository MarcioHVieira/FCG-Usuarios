using Fcg.Common.Email;
using Usuarios.Api.Application.Interfaces;
using Usuarios.Api.Application.Services;
using Usuarios.Api.Application.Services.Jwt;
using Usuarios.Api.Domain.Interfaces;
using Usuarios.Api.Infrastructure.Data;

namespace Usuarios.Api.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static WebApplicationBuilder RegisterDependencies(this WebApplicationBuilder builder)
        {
            var emailSettings = builder.Configuration.GetSection("EmailSettings").Get<EmailSettings>()
                                ?? throw new InvalidOperationException("Configuração de e-mail inválida.");

            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

            if (jwtSettings is null || string.IsNullOrEmpty(jwtSettings.SecretKey))
                throw new InvalidOperationException("Configuração JWT inválida.");

            builder.Services.AddSingleton(jwtSettings);

            builder.Services.AddScoped<IJwtService, JwtService>();

            builder.Services.AddSingleton(emailSettings);
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IModeloEmail, ModeloEmail>();

            builder.Services.AddScoped<IModeloEmail, ModeloEmail>();

            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            builder.Services.AddScoped<IUsuarioService, UsuarioService>();

            builder.Services.AddScoped<UsuariosDbContext>();

            builder.Services.AddHttpContextAccessor();

            return builder;
        }
    }
}
