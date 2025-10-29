using Fcg.Common.Entities;
using Fcg.Common.Enums;
using Isopoh.Cryptography.Argon2;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Usuarios.Api.Domain.Entities
{
    public class Usuario : EntityBase
    {
        public string Nome { get; private set; }
        public string Apelido { get; private set; }
        public string Email { get; private set; }
        public string Senha { get; private set; }
        public string Salt { get; private set; }
        public Role Role { get; private set; }
        public int TentativasLogin { get; private set; }
        public string CodigoAtivacao { get; private set; }
        public string CodigoValidacao { get; private set; }

        // EF
        protected Usuario() { }

        private Usuario(Guid id, string nome, string apelido, string email, string senhaHash, string salt)
        {
            Id = id;
            Nome = nome;
            Apelido = apelido;
            Email = email;
            Senha = senhaHash;
            Salt = salt;
            CodigoAtivacao = string.Empty;
            CodigoValidacao = string.Empty;
        }

        public void AdicionarTentativaLoginErrada() => TentativasLogin++;
        public void ZerarTentativasLoginErrada() => TentativasLogin = 0;
        public void LimparCodigoAtivacao() => CodigoAtivacao = string.Empty;
        public void LimparCodigoValidacao() => CodigoValidacao = string.Empty;
        public void TornarAdministrador() => Role = Role.Administrador;
        public void TornarUsuario() => Role = Role.Usuario;

        public static Usuario CriarAlterar(Guid? id, string nome, string apelido, string email, string senha)
        {
            if (!ValidarEmail(email))
                throw new InvalidOperationException("Endereço de e-mail inválido.");

            string senhaHash = string.Empty, salt = string.Empty;

            if (id == null)
            {
                if (!ValidarSenhaForte(senha))
                    throw new InvalidOperationException("A senha deve conter pelo menos uma letra, um número e um caractere especial.");

                (senhaHash, salt) = GerarHashSenha(senha);
            }

            return new Usuario(id ?? Guid.NewGuid(), nome, apelido, email, senhaHash, salt);
        }

        public void AlterarSenha(string novaSenha)
        {
            if (!ValidarSenhaForte(novaSenha))
                throw new InvalidOperationException("A senha deve conter pelo menos uma letra, um número e um caractere especial.");

            (Senha, Salt) = GerarHashSenha(novaSenha);
        }

        public bool ValidarSenha(string senha) => Argon2.Verify(Senha, senha + Salt);

        public void GerarCodigoAtivacao() => CodigoAtivacao = GerarCodigoAleatorio(8);

        public void GerarCodigoValidacao() => CodigoValidacao = Guid.NewGuid().ToString().ToUpper();

        public bool ValidarCodigoAtivacao(string codigo) => ValidarCodigo(codigo, CodigoAtivacao);

        public bool ValidarCodigoValidacao(string codigo) => ValidarCodigo(codigo, CodigoValidacao, true);

        #region Métodos Privados
        private static bool ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            try
            {
                var endereco = new MailAddress(email);
                return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            }
            catch
            {
                return false;
            }
        }

        private static bool ValidarSenhaForte(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha)) return false;

            bool temLetra = Regex.IsMatch(senha, @"[a-zA-Z]");
            bool temNumero = Regex.IsMatch(senha, @"\d");
            bool temEspecial = Regex.IsMatch(senha, @"[!@#$%^&*(),.?""{}|<>]");

            return temLetra && temNumero && temEspecial;
        }

        private bool ValidarCodigo(string codigo, string codigoReferencia, bool verificarFormatoGuid = false)
        {
            if (string.IsNullOrWhiteSpace(codigo) || (verificarFormatoGuid && !Guid.TryParse(codigo, out _)))
                return false;

            if (codigoReferencia == codigo)
            {
                Ativar();
                return true;
            }

            Desativar();
            return false;
        }

        private static string GerarCodigoAleatorio(int tamanho)
        {
            const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();

            return new string(Enumerable.Repeat(caracteres, tamanho)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static (string hash, string salt) GerarHashSenha(string senha)
        {
            var salt = Guid.NewGuid().ToString();
            var hash = Argon2.Hash(senha + salt);
            return (hash, salt);
        }
        #endregion
    }
}