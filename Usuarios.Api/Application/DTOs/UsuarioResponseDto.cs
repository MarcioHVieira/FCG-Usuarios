using Fcg.Common.Enums;

namespace Usuarios.Api.Application.DTOs
{
    public class UsuarioResponseDto
    {
        public Guid Id { get; set; }
        public required string Nome { get; set; }
        public required string Apelido { get; set; }
        public required string Email { get; set; }
        public required Role Role { get; set; }
        public required string Status { get; set; }
    }
}
