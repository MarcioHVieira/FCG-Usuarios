using Usuarios.Api.Domain.Entities;
using Usuarios.Api.Application.DTOs;

namespace Usuarios.Api.Application.Mappers
{
    public static class UsuarioMapper
    {
        public static Usuario ToDomain(this UsuarioAdicionarDto usuarioDto)
        {
            var usuario = Usuario.CriarAlterar(null, usuarioDto.Nome, usuarioDto.Apelido,
                                               usuarioDto.Email, usuarioDto.Senha);
            usuario.GerarCodigoAtivacao();

            return usuario;
        }

        public static Usuario ToDomain(this UsuarioAlterarDto usuarioDto)
        {
            return Usuario.CriarAlterar(usuarioDto.Id, usuarioDto.Nome, usuarioDto.Apelido, 
                                        usuarioDto.Email, string.Empty);
        }

        public static UsuarioResponseDto ToDto(this Usuario usuario)
        {
            return new UsuarioResponseDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Apelido = usuario.Apelido,
                Email = usuario.Email,
                Role = usuario.Role,
                Status = usuario.Ativo ? "Ativado" : "Desativado"
            };
        }
    }
}
