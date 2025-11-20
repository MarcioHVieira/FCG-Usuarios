using Fcg.Common.Email;
using Fcg.Common.Extensions;
using Fcg.Common.PdfGenerator.Models;
using Fcg.Common.PdfGenerator.Services;
using Usuarios.Api.Application.Constants;
using Usuarios.Api.Application.DTOs;
using Usuarios.Api.Application.Interfaces;
using Usuarios.Api.Application.Mappers;
using Usuarios.Api.Application.Services.Jwt;
using Usuarios.Api.Domain.Entities;
using Usuarios.Api.Domain.Interfaces;

namespace Usuarios.Api.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IModeloEmail _email;
        private readonly IJwtService _jwtService;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(IUsuarioRepository usuarioRepository, 
                              IModeloEmail email, 
                              IJwtService jwtService, 
                              ILogger<UsuarioService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _email = email;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<string> Login(LoginDto login)
        {
            try
            {
                var usuario = await ValidarAcesso(login.Apelido, login.Senha, false);

                return _jwtService.GerarToken(usuario.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogService(
                    ServiceConstants.ServiceName, 
                    "Login", 
                    "Erro", 
                    "Erro ao realizar login",
                    new { Usuario = login.Apelido }, ex);
                throw;
            }
        }

        public async Task<string> LoginAtivacao(LoginAtivacaoDto login)
        {
            try
            {
                var usuario = await ValidarAcesso(login.Apelido, login.Senha, true);

                if (!usuario.ValidarCodigoAtivacao(login.CodigoAtivacao))
                    throw new InvalidOperationException("Código de ativação inválido");

                usuario.ZerarTentativasLoginErrada();
                usuario.LimparCodigoAtivacao();
                usuario.Ativar();
                await _usuarioRepository.Alterar(usuario, true);

                return _jwtService.GerarToken(usuario.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogService(
                    ServiceConstants.ServiceName, 
                    "LoginAtivacao", 
                    "Erro", 
                    "Erro ao tentar ativar o login", 
                    new { Usuario = login.Apelido }, ex);
                throw;
            }
        }

        public async Task<string> LoginNovaSenha(LoginNovaSenhaDto login)
        {
            try
            {
                var usuario = await _usuarioRepository.ObterPorApelido(login.Apelido)
                    ?? throw new InvalidOperationException("Usuário ou Código inválido");

                if (!usuario.ValidarCodigoValidacao(login.CodigoValidacao))
                    throw new InvalidOperationException("Usuário ou Código inválido");

                usuario.AlterarSenha(login.NovaSenha);
                usuario.ZerarTentativasLoginErrada();
                usuario.LimparCodigoValidacao();
                usuario.Ativar();
                await _usuarioRepository.Alterar(usuario, true);

                return _jwtService.GerarToken(usuario.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogService(
                    ServiceConstants.ServiceName, 
                    "LoginNovaSenha", 
                    "Erro", 
                    "Erro ao tentar criar nova senha", 
                    new { Usuario = login.Apelido }, ex);
                throw;
            }
        }

        public async Task SolicitarNovaSenha(string email)
        {
            try
            {
                var usuario = await _usuarioRepository.ObterPorEmail(email)
                    ?? throw new KeyNotFoundException("E-mail não encontrado");

                usuario.GerarCodigoValidacao();
                await _usuarioRepository.Alterar(usuario);

                await _email.SolicitacaoNovaSenha(usuario.Email, usuario.Nome, usuario.CodigoValidacao);
            }
            catch (Exception ex)
            {
                _logger.LogService(
                    ServiceConstants.ServiceName, 
                    "SolicitarNovaSenha", 
                    "Erro", 
                    "Erro ao solicitar nova senha", 
                    new { Email = email }, ex);
                throw;
            }
        }

        public async Task SolicitarReativacao(string email)
        {
            try
            {
                var usuario = await _usuarioRepository.ObterPorEmail(email)
                    ?? throw new KeyNotFoundException("E-mail não encontrado");

                if (usuario.Ativo)
                    throw new InvalidOperationException("Sua conta já se encontra ativa");

                usuario.GerarCodigoAtivacao();
                await _usuarioRepository.Alterar(usuario);

                await _email.CodigoAtivacao(usuario.Email, usuario.Nome, usuario.CodigoAtivacao);
            }
            catch (Exception ex)
            {
                _logger.LogService(
                    ServiceConstants.ServiceName,
                    "SolicitarReativacao",
                    "Erro",
                    "Erro ao tentar solicitar reativação",
                    new { Email = email }, ex);
                throw;
            }
        }

        public async Task ReenviarCodigoAtivacao(string email)
        {
            try
            {
                var usuario = await _usuarioRepository.ObterPorEmail(email)
                    ?? throw new KeyNotFoundException("O e-mail informado não foi encontrado");

                if (string.IsNullOrEmpty(usuario.CodigoAtivacao))
                    throw new KeyNotFoundException("Você não tem um código de ativação");

                await _email.CodigoAtivacao(usuario.Email, usuario.Nome, usuario.CodigoAtivacao);
            }
            catch (Exception ex)
            {
                _logger.LogService(
                    ServiceConstants.ServiceName,
                    "ReenviarCodigoAtivacao",
                    "Erro",
                    "Erro ao tentar reenviar código de ativação",
                    new { Email = email }, ex);
                throw;
            }
        }

        public async Task ReenviarCodigoValidacao(string email)
        {
            try
            {
                var usuario = await _usuarioRepository.ObterPorEmail(email)
                    ?? throw new KeyNotFoundException("O e-mail informado não foi encontrado");

                if (string.IsNullOrEmpty(usuario.CodigoValidacao))
                    throw new KeyNotFoundException("Você não tem um código de validação");

                await _email.SolicitacaoNovaSenha(usuario.Email, usuario.Nome, usuario.CodigoValidacao);
            }
            catch (Exception ex)
            {
                _logger.LogService(
                    ServiceConstants.ServiceName,
                    "ReenviarCodigoValidacao",
                    "Erro",
                    "Erro ao tentar reenviar código de validação",
                    new { Email = email }, ex);
                throw;
            }
        }

        public async Task<UsuarioResponseDto> ObterUsuario(Guid usuarioId)
        {
            try
            {
                var usuario = await _usuarioRepository.ObterPorId(usuarioId)
                    ?? throw new KeyNotFoundException("Usuário não encontrado com o Id informado");

                return usuario.ToDto();
            }
            catch (Exception ex)
            {
                _logger.LogService(
                    ServiceConstants.ServiceName,
                    "ObterUsuario",
                    "Erro",
                    "Erro ao tentar obter o usuário",
                    new { UsuarioId = usuarioId }, ex);
                throw;
            }
        }

        public async Task<UsuarioResponseDto> ObterUsuarioPorApelido(string apelido)
        {
            try
            {
                var usuario = await _usuarioRepository.ObterPorApelido(apelido)
                    ?? throw new KeyNotFoundException("Usuário não encontrado com o Apelido informado");

                return usuario.ToDto();
            }
            catch (Exception ex)
            {
                _logger.LogService(
                    ServiceConstants.ServiceName,
                    "ObterUsuarioPorApelido",
                    "Erro",
                    "Erro ao tentar obter usuário pelo apelido",
                    new { Usuario = apelido }, ex);
                throw;
            }
        }

        public async Task<UsuarioResponseDto> ObterUsuarioPorEmail(string email)
        {
            try
            {
                var usuario = await _usuarioRepository.ObterPorEmail(email)
                    ?? throw new KeyNotFoundException("Usuário não encontrado com o e-mail informado");

                return usuario.ToDto();
            }
            catch (Exception ex)
            {
                _logger.LogService(
                    ServiceConstants.ServiceName,
                    "ObterUsuarioPorEmail",
                    "Erro",
                    "Erro ao tentar obter usuário por email",
                    new { Email = email }, ex);
                throw;
            }
        }

        public async Task<IEnumerable<UsuarioResponseDto>> ObterUsuarios()
        {
            try
            {
                var usuarios = await _usuarioRepository.ObterTodos();

                return usuarios.Select(u => u.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogService(
                    ServiceConstants.ServiceName,
                    "ObterUsuarios",
                    "Erro",
                    "Erro ao tentar obter todos os usuários",
                    null, ex);
                throw;
            }
        }

        public async Task<IEnumerable<UsuarioResponseDto>> ObterUsuariosAtivos()
        {
            try
            {
                var usuarios = await _usuarioRepository.ObterTodosAtivos();

                return usuarios.Select(u => u.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogService(
                    ServiceConstants.ServiceName,
                    "ObterUsuariosAtivos",
                    "Erro",
                    "Erro ao tentar obter os usuários ativos",
                    null, ex);
                throw;
            }
        }

        public async Task AdicionarUsuario(UsuarioAdicionarDto usuarioDto)
        {
            try
            {
                var usuario = usuarioDto.ToDomain();
                await _usuarioRepository.Adicionar(usuario);

                var pdfService = new PdfGeneratorService();
                var modelo = new CertificadoModel
                {
                    NomeUsuario = usuario.Nome,
                    DataCadastro = DateTime.Now,
                };
                var pdfBytes = pdfService.GerarPdf(modelo, DocumentoModelo.CartaBoasVindas);

                await _email.BoasVindas(usuario.Email, usuario.Nome, pdfBytes);

                await _email.CodigoAtivacao(usuario.Email, usuario.Nome, usuario.CodigoAtivacao);
            }
            catch (Exception ex)
            {
                _logger.LogService(
                    ServiceConstants.ServiceName,
                    "AdicionarUsuario",
                    "Erro",
                    "Erro ao tentar adicionar usuário",
                    new { Email = usuarioDto.Email, Usuario = usuarioDto.Apelido }, ex);
                throw;
            }
        }

        public async Task AlterarUsuario(UsuarioAlterarDto usuarioDto)
        {
            try
            {
                await _usuarioRepository.Alterar(usuarioDto.ToDomain());
            }
            catch (Exception ex)
            {
                _logger.LogService(
                    ServiceConstants.ServiceName,
                    "AlterarUsuario",
                    "Erro",
                    "Erro ao tentar alterar usuário",
                    new { UsuarioId = usuarioDto.Id, Email = usuarioDto.Email, Usuario = usuarioDto.Apelido }, ex);
                throw;
            }
        }

        public async Task AlterarSenha(Guid usuarioId, string novaSenha)
        {
            try
            {
                var usuario = await _usuarioRepository.ObterPorId(usuarioId);
                usuario?.AlterarSenha(novaSenha);

                await _usuarioRepository.Alterar(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogService(
                    ServiceConstants.ServiceName,
                    "AlterarSenha",
                    "Erro",
                    "Erro ao tentar alterar senha",
                    new { UsuarioId = usuarioId }, ex);
                throw;
            }
        }

        public async Task AtivarUsuario(Guid usuarioId)
        {
            try
            {
                await _usuarioRepository.Ativar(usuarioId);
            }
            catch (Exception ex)
            {
                _logger.LogService(
                    ServiceConstants.ServiceName,
                    "AtivarUsuario",
                    "Erro",
                    "Erro ao tentar ativar usuário",
                    new { UsuarioId = usuarioId }, ex);
                throw;
            }
        }

        public async Task DesativarUsuario(Guid usuarioId)
        {
            try
            {
                await _usuarioRepository.Desativar(usuarioId);
            }
            catch (Exception ex)
            {
                _logger.LogService(
                    ServiceConstants.ServiceName,
                    "DesativarUsuario",
                    "Erro",
                    "Erro ao tentar desativar usuário",
                    new { UsuarioId = usuarioId }, ex);
                throw;
            }
        }

        public async Task TornarUsuario(Guid usuarioId)
        {
            try
            {
                await AlterarTipoUsuario(usuarioId, u => u.TornarUsuario());
            }
            catch (Exception ex)
            {
                _logger.LogService(
                    ServiceConstants.ServiceName,
                    "TornarUsuario",
                    "Erro",
                    "Erro ao tentar alterar o tipo para usuário",
                    new { UsuarioId = usuarioId }, ex);
                throw;
            }
        }

        public async Task TornarAdministrador(Guid usuarioId)
        {
            try
            {
                await AlterarTipoUsuario(usuarioId, u => u.TornarAdministrador());
            }
            catch (Exception ex)
            {
                _logger.LogService(
                    ServiceConstants.ServiceName,
                    "TornarAdministrador",
                    "Erro",
                    "Erro ao tentar alterar o tipo para administrador",
                    new { UsuarioId = usuarioId }, ex);
                throw;
            }
        }

        #region Métodos Privados
        private async Task AlterarTipoUsuario(Guid usuarioId, Action<Usuario> alterarFunc)
        {
            try
            {
                var usuario = await _usuarioRepository.ObterPorId(usuarioId)
                    ?? throw new KeyNotFoundException("Usuário não encontrado com o Id informado");

                alterarFunc(usuario);
                await _usuarioRepository.Alterar(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogService(
                    ServiceConstants.ServiceName,
                    "AlterarTipoUsuario",
                    "Erro",
                    "Erro ao tentar alterar o tipo de usuário",
                    new { UsuarioId = usuarioId }, ex);
                throw;
            }
        }

        private async Task<Usuario> ValidarAcesso(string apelido, string senha, bool ehLoginAtivacao)
        {
            var usuario = await _usuarioRepository.ObterPorApelido(apelido)
                ?? throw new UnauthorizedAccessException("Usuário ou senha inválida");

            if (!ehLoginAtivacao)
            {
                if (!usuario.Ativo)
                    throw new UnauthorizedAccessException("Sua conta está bloqueada. Solicite a reativação da conta ou redefinir sua senha.");
            }

            if (!usuario.ValidarSenha(senha))
            {
                if (usuario.Ativo)
                {
                    usuario.AdicionarTentativaLoginErrada();
                    await _usuarioRepository.Alterar(usuario, true);

                    if (usuario.TentativasLogin >= 3)
                    {
                        await _usuarioRepository.Desativar(usuario.Id);

                        throw new UnauthorizedAccessException("Usuário bloqueado por excesso de tentativas. Solicite a reativação da conta ou redefinir sua senha.");
                    }
                }

                throw new UnauthorizedAccessException("Usuário ou senha inválida.");
            }

            if (usuario.TentativasLogin > 0)
            {
                usuario.ZerarTentativasLoginErrada();
                await _usuarioRepository.Alterar(usuario);
            }

            return usuario;
        }
        #endregion
    }
}
