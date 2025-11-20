using System.ComponentModel.DataAnnotations;

namespace Usuarios.Api.Application.DTOs
{
    public class LoginAtivacaoDto
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public required string Apelido { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public required string Senha { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(8, ErrorMessage = "O campo {0} precisa ter {1} caracteres", MinimumLength = 8)]
        public required string CodigoAtivacao { get; set; }
    }
}
