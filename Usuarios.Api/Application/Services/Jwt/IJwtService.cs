using Usuarios.Api.Application.DTOs;

namespace Usuarios.Api.Application.Services.Jwt
{
    public interface IJwtService
    {
        string GerarToken(UsuarioResponseDto usuarioDto);
    }
}
