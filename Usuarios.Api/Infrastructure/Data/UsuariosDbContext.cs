using Usuarios.Api.Domain.Entities;
using Fcg.Common.Entities;
using Fcg.Common.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Usuarios.Api.Infrastructure.Data
{
    public class UsuariosDbContext : DbContext, IUnitOfWork
    {
        public DbSet<Usuario> Usuarios { get; set; }

        public UsuariosDbContext(DbContextOptions<UsuariosDbContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("usuarios");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsuariosDbContext).Assembly);
        }

        public async Task Salvar(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<EntityBase>())
            {
                if (entry.State == EntityState.Added)
                    entry.Property("DataCadastro").CurrentValue = DateTime.UtcNow;

                if (entry.State == EntityState.Modified)
                    entry.Property("DataCadastro").IsModified = false;
            }

            foreach (var entry in ChangeTracker.Entries<Usuario>())
            {
                var senhaProperty = entry.Property(nameof(Usuario.Senha));
                if (entry.State == EntityState.Modified && senhaProperty.CurrentValue is string senha && string.IsNullOrEmpty(senha))
                    senhaProperty.IsModified = false;

                var saltProperty = entry.Property(nameof(Usuario.Salt));
                if (entry.State == EntityState.Modified && saltProperty.CurrentValue is string salt && string.IsNullOrEmpty(salt))
                    saltProperty.IsModified = false;

                var codigoAtivacaoProperty = entry.Property(nameof(Usuario.CodigoAtivacao));
                if (entry.State == EntityState.Modified && codigoAtivacaoProperty.CurrentValue is null)
                    codigoAtivacaoProperty.IsModified = false;

                var codigoValidacaoProperty = entry.Property(nameof(Usuario.CodigoValidacao));
                if (entry.State == EntityState.Modified && codigoValidacaoProperty.CurrentValue is null)
                    codigoValidacaoProperty.IsModified = false;
            }

            var salvo = await base.SaveChangesAsync(cancellationToken) > 0;

            if (!salvo)
                throw new DbUpdateException("Houve um erro ao tentar persistir os dados");
        }
    }
}
