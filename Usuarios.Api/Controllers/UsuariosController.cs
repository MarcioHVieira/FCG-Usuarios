using Usuarios.Api.Application.DTOs;
using Usuarios.Api.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Fcg.Common.Controllers;

namespace Usuarios.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuariosController : MainController
    {
        private readonly IUsuarioService _usuario;

        public UsuariosController(IUsuarioService usuario)
        {
            _usuario = usuario;
        }

        [HttpPost("Login")]
        [SwaggerOperation(Summary = "Login do usuário", 
                          Description = "Realiza login e retorna um token JWT para autenticação.")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var token = await _usuario.Login(login);
            return CustomResponse(token);
        }

        [HttpPost("LoginAtivacao")]
        [SwaggerOperation(Summary = "Login de ativação", 
                          Description = "Realiza login para ativação de conta utilizando o código de ativação enviado por e-mail.")]
        public async Task<IActionResult> LoginAtivacao([FromBody] LoginAtivacaoDto login)
        {
            var token = await _usuario.LoginAtivacao(login);
            return CustomResponse(token);
        }

        [HttpPost("LoginNovaSenha")]
        [SwaggerOperation(Summary = "Login para redefinição de senha", 
                          Description = "Realiza login para redefinição de senha utilizando o código de validação enviado por e-mail.")]
        public async Task<IActionResult> LoginNovaSenha([FromBody] LoginNovaSenhaDto login)
        {
            var token = await _usuario.LoginNovaSenha(login);
            return CustomResponse(token);
        }

        [HttpGet("SolicitarNovaSenha")]
        [SwaggerOperation(Summary = "Solicitação de nova senha", 
                          Description = "Envia um código de validação por e-mail para redefinição de senha.")]
        public async Task<IActionResult> SolicitarNovaSenha(string email)
        {
            await _usuario.SolicitarNovaSenha(email);
            return CustomResponse("Solicitação realizada com sucesso.");
        }

        [HttpGet("SolicitarReativacao")]
        [SwaggerOperation(Summary = "Solicitação de reativação", 
                          Description = "Envia um código de reativação de conta para o e-mail do usuário.")]
        public async Task<IActionResult> SolicitarReativacao(string email)
        {
            await _usuario.SolicitarReativacao(email);
            return CustomResponse("Solicitação realizada com sucesso. Você receberá um e-mail contendo o código de reativação.");
        }

        [HttpGet("ReenviarCodigoAtivacao")]
        [SwaggerOperation(Summary = "Reenvio de código de ativação", 
                          Description = "Reenvia o código de ativação da conta para o e-mail do usuário.")]
        public async Task<IActionResult> ReenviarCodigoAtivacao(string email)
        {
            await _usuario.ReenviarCodigoAtivacao(email);
            return CustomResponse("Código de ativação reenviado com sucesso.");
        }

        [HttpGet("ReenviarCodigoValidacao")]
        [SwaggerOperation(Summary = "Reenvio de código de validação", 
                          Description = "Reenvia o código de validação para recuperação de senha.")]
        public async Task<IActionResult> ReenviarCodigoValidacao(string email)
        {
            await _usuario.ReenviarCodigoValidacao(email);
            return CustomResponse("Código de validação reenviado com sucesso.");
        }

        [Authorize(Roles = "Usuario,Administrador")]
        [HttpGet("ObterUsuario")]
        [SwaggerOperation(Summary = "Obter usuário autenticado", 
                          Description = "Retorna os dados do usuário atualmente autenticado.")]
        public async Task<IActionResult> ObterUsuario()
        {
            var usuario = await _usuario.ObterUsuario(ObterIdUsuarioLogado());
            return CustomResponse(usuario);
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet("ObterUsuarioPorApelido")]
        [SwaggerOperation(Summary = "Obter usuário por apelido", 
                          Description = "Busca um usuário cadastrado pelo apelido.")]
        public async Task<IActionResult> ObterUsuarioPorApelido(string apelido)
        {
            var usuario = await _usuario.ObterUsuarioPorApelido(apelido);
            return CustomResponse(usuario);
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet("ObterUsuarioPorEmail")]
        [SwaggerOperation(Summary = "Obter usuário por e-mail", 
                          Description = "Busca um usuário cadastrado pelo e-mail.")]
        public async Task<IActionResult> ObterUsuarioPorEmail(string email)
        {
            var usuario = await _usuario.ObterUsuarioPorEmail(email);
            return CustomResponse(usuario);
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet("ObterUsuarios")]
        [SwaggerOperation(Summary = "Lista de usuários", 
                          Description = "Retorna todos os usuários cadastrados (ativos e não ativos).")]
        public async Task<IActionResult> ObterUsuarios()
        {
            var usuarios = await _usuario.ObterUsuarios();
            return usuarios.Any()
                ? CustomResponse(usuarios)
                : CustomResponse("Nenhum usuário encontrado", StatusCodes.Status404NotFound);
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet("ObterUsuariosAtivos")]
        [SwaggerOperation(Summary = "Lista de usuários ativos", 
                          Description = "Retorna todos os usuários ativos cadastrados no sistema.")]
        public async Task<IActionResult> ObterUsuariosAtivos()
        {
            var usuarios = await _usuario.ObterUsuariosAtivos();
            return usuarios.Any()
                ? CustomResponse(usuarios)
                : CustomResponse("Nenhum usuário encontrado", StatusCodes.Status404NotFound);
        }

        [HttpPost("AdicionarUsuario")]
        [SwaggerOperation(Summary = "Criação de usuário", 
                          Description = "Adiciona um novo usuário e envia um código de ativação por e-mail.")]
        public async Task<IActionResult> AdicionarUsuario([FromBody] UsuarioAdicionarDto usuario)
        {
            if (!ValidarModelo())
                return CustomResponse();

            await _usuario.AdicionarUsuario(usuario);
            return CustomResponse("Usuário adicionado com sucesso. Você receberá um e-mail contendo o código de ativação da sua conta.");
        }

        [Authorize(Roles = "Usuario,Administrador")]
        [HttpPut("AlterarUsuario")]
        [SwaggerOperation(Summary = "Alteração de usuário", 
                          Description = "Permite que o usuário altere seus dados ou um administrador altere os dados de outros usuários.")]
        public async Task<IActionResult> AlterarUsuario(UsuarioAlterarDto usuario)
        {
            if (!ValidarModelo())
                return CustomResponse();

            if (!ValidarPermissao(usuario.Id))
                return CustomResponse();

            await _usuario.AlterarUsuario(usuario);
            return CustomResponse("Usuário alterado com sucesso.");
        }

        [Authorize(Roles = "Usuario,Administrador")]
        [HttpPut("AlterarSenha")]
        [SwaggerOperation(Summary = "Alteração de senha", 
                          Description = "Permite que um usuário altere sua senha.")]
        public async Task<IActionResult> AlterarSenha(string novaSenha)
        {
            if (!ValidarModelo())
                return CustomResponse();

            await _usuario.AlterarSenha(ObterIdUsuarioLogado(), novaSenha);
            return CustomResponse("Senha alterada com sucesso.");
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("AtivarUsuario")]
        [SwaggerOperation(Summary = "Ativação de usuário",
                          Description = "Permite que um administrador ative uma conta.")]
        public async Task<IActionResult> AtivarUsuario(Guid usuarioId)
        {
            if (!ValidarPermissao(usuarioId))
                return CustomResponse();

            await _usuario.AtivarUsuario(usuarioId);
            return CustomResponse("Usuário ativado com sucesso.");
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("DesativarUsuario")]
        [SwaggerOperation(Summary = "Desativação de usuário", 
                          Description = "Permite que um administrador desative uma conta.")]
        public async Task<IActionResult> DesativarUsuario(Guid usuarioId)
        {
            await _usuario.DesativarUsuario(usuarioId);
            return CustomResponse("Usuário desativado com sucesso.");
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("TornarUsuario")]
        [SwaggerOperation(Summary = "Alterar perfil para Usuário", 
                          Description = "Permite que um administrador altere o perfil de um usuário para 'Usuário'.")]
        public async Task<IActionResult> TornarUsuario(Guid usuarioId)
        {
            await _usuario.TornarUsuario(usuarioId);
            return CustomResponse("Usuário alterado para o perfil de 'Usuário'.");
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("TornarAdministrador")]
        [SwaggerOperation(Summary = "Alterar perfil para Administrador", 
                          Description = "Permite que um administrador altere o perfil de um usuário para 'Administrador'.")]
        public async Task<IActionResult> TornarAdministrador(Guid usuarioId)
        {
            await _usuario.TornarAdministrador(usuarioId);
            return CustomResponse("Usuário alterado para o perfil de 'Administrador'.");
        }
    }
}
