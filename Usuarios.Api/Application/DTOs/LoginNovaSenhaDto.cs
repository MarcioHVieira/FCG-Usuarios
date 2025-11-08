using System.ComponentModel.DataAnnotations;

namespace Usuarios.Api.Application.DTOs
{
    public class LoginNovaSenhaDto
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public required string Apelido { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public required string NovaSenha { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(36, ErrorMessage = "O campo {0} precisa ter {1} caracteres", MinimumLength = 36)]
        public required string CodigoValidacao { get; set; }
    }
}
