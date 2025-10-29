using Fcg.Common.Repositories;
using Usuarios.Api.Domain.Entities;
using Usuarios.Api.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Usuarios.Api.Infrastructure.Data
{
    public class UsuarioRepository : RepositoryBase<Usuario, UsuariosDbContext>, IUsuarioRepository
    {
        private readonly UsuariosDbContext _context;

        public UsuarioRepository(UsuariosDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ExisteApelido(string apelido, Guid usuarioId)
        {
            return await _context.Usuarios.AsNoTracking()
                .Where(u => u.Id != usuarioId && u.Apelido == apelido).AnyAsync();
        }

        public async Task<bool> ExisteEmail(string email, Guid usuarioId)
        {
            return await _context.Usuarios.AsNoTracking()
                .Where(u => u.Id != usuarioId && u.Email == email).AnyAsync();
        }

        public async Task<Usuario?> ObterPorApelido(string apelido)
        {
            return await _context.Usuarios.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Apelido == apelido);
        }

        public async Task<Usuario?> ObterPorEmail(string email)
        {
            return await _context.Usuarios.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public override async Task Adicionar(Usuario usuario)
        {
            usuario.Desativar();

            _context.Usuarios.Add(usuario);
            
            await _context.Salvar();
        }
    }
}
