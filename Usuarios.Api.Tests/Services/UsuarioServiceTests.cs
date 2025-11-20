using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Usuarios.Api.Application.Services;
using Usuarios.Api.Application.DTOs;
using Usuarios.Api.Domain.Entities;
using Usuarios.Api.Domain.Interfaces;
using Usuarios.Api.Application.Services.Jwt;
using Fcg.Common.Email;

namespace Usuarios.Api.Tests.Services
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock = new();
        private readonly Mock<IModeloEmail> _emailMock = new();
        private readonly Mock<IJwtService> _jwtServiceMock = new();
        private readonly Mock<ILogger<UsuarioService>> _loggerMock = new();
        private readonly UsuarioService _service;

        public UsuarioServiceTests()
        {
            _service = new UsuarioService(
                _usuarioRepositoryMock.Object,
                _emailMock.Object,
                _jwtServiceMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Login_DeveRetornarToken_QuandoCredenciaisValidas()
        {
            // Arrange
            var loginDto = new LoginDto { Apelido = "Marcio", Senha = "Senha@123" };
            var usuario = Usuario.CriarAlterar(null, "Marcio Henrique", "Marcio", "marcio@teste.com", "Senha@123");
            usuario.Ativar();
            _usuarioRepositoryMock.Setup(r => r.ObterPorApelido("Marcio")).ReturnsAsync(usuario);
            _jwtServiceMock.Setup(j => j.GerarToken(It.IsAny<UsuarioResponseDto>())).Returns("token");
            _usuarioRepositoryMock.Setup(r => r.Alterar(usuario, true)).Returns(Task.CompletedTask);

            // Act
            var result = await _service.Login(loginDto);

            // Assert
            Assert.Equal("token", result);
        }

        [Fact]
        public async Task Login_DeveLancarExcecao_QuandoUsuarioNaoEncontrado()
        {
            // Arrange
            var loginDto = new LoginDto { Apelido = "Marcio", Senha = "Senha@123" };
            _usuarioRepositoryMock.Setup(r => r.ObterPorApelido("Marcio")).ReturnsAsync((Usuario)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.Login(loginDto));
        }

        [Fact]
        public async Task LoginAtivacao_DeveRetornarToken_QuandoAtivacaoValida()
        {
            // Arrange
            var usuario = Usuario.CriarAlterar(null, "Marcio Henrique", "Marcio", "marcio@teste.com", "Senha@123");
            usuario.GerarCodigoAtivacao();
            usuario.Ativar();

            var loginDto = new LoginAtivacaoDto
            {
                Apelido = "Marcio",
                Senha = "Senha@123",
                CodigoAtivacao = usuario.CodigoAtivacao
            };

            _usuarioRepositoryMock.Setup(r => r.ObterPorApelido("Marcio")).ReturnsAsync(usuario);
            _jwtServiceMock.Setup(j => j.GerarToken(It.IsAny<UsuarioResponseDto>())).Returns("token");
            _usuarioRepositoryMock.Setup(r => r.Alterar(usuario, true)).Returns(Task.CompletedTask);

            // Act
            var result = await _service.LoginAtivacao(loginDto);

            // Assert
            Assert.Equal("token", result);
        }

        [Fact]
        public async Task LoginAtivacao_DeveLancarExcecao_QuandoCodigoAtivacaoInvalido()
        {
            // Arrange
            var loginDto = new LoginAtivacaoDto { Apelido = "Marcio", Senha = "Senha@123", CodigoAtivacao = "wrong" };
            var usuario = Usuario.CriarAlterar(null, "Marcio Henrique", "Marcio", "marcio@teste.com", "Senha@123");
            usuario.GerarCodigoAtivacao();
            _usuarioRepositoryMock.Setup(r => r.ObterPorApelido("Marcio")).ReturnsAsync(usuario);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.LoginAtivacao(loginDto));
        }

        [Fact]
        public async Task LoginNovaSenha_DeveRetornarToken_QuandoValidacaoValida()
        {
            // Arrange
            var usuario = Usuario.CriarAlterar(null, "Marcio Henrique", "Marcio", "marcio@teste.com", "Senha@123");
            usuario.GerarCodigoValidacao();
            _usuarioRepositoryMock.Setup(r => r.ObterPorApelido("Marcio")).ReturnsAsync(usuario);

            var loginDto = new LoginNovaSenhaDto
            {
                Apelido = "Marcio",
                CodigoValidacao = usuario.CodigoValidacao,
                NovaSenha = "NovaSenha@123"
            };

            _jwtServiceMock.Setup(j => j.GerarToken(It.IsAny<UsuarioResponseDto>())).Returns("token");
            _usuarioRepositoryMock.Setup(r => r.Alterar(usuario, true)).Returns(Task.CompletedTask);

            // Act
            var result = await _service.LoginNovaSenha(loginDto);

            // Assert
            Assert.Equal("token", result);
        }

        [Fact]
        public async Task LoginNovaSenha_DeveLancarExcecao_QuandoCodigoValidacaoInvalido()
        {
            // Arrange
            var usuario = Usuario.CriarAlterar(null, "Marcio Henrique", "Marcio", "marcio@teste.com", "Senha@123");
            usuario.GerarCodigoValidacao();
            _usuarioRepositoryMock.Setup(r => r.ObterPorApelido("Marcio")).ReturnsAsync(usuario);

            var loginDto = new LoginNovaSenhaDto
            {
                Apelido = "Marcio",
                CodigoValidacao = "wrong",
                NovaSenha = "NovaSenha@123"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.LoginNovaSenha(loginDto));
        }
    }
}
