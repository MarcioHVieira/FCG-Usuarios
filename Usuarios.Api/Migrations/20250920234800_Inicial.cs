using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Usuarios.Api.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "usuarios");

            migrationBuilder.CreateTable(
                name: "Usuarios",
                schema: "usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "varchar(100)", nullable: false),
                    Apelido = table.Column<string>(type: "varchar(30)", nullable: false),
                    Email = table.Column<string>(type: "varchar(200)", nullable: false),
                    Senha = table.Column<string>(type: "nvarchar(1000)", nullable: false),
                    Salt = table.Column<string>(type: "varchar(50)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    TentativasLogin = table.Column<int>(type: "int", nullable: false),
                    CodigoAtivacao = table.Column<string>(type: "varchar(8)", nullable: false),
                    CodigoValidacao = table.Column<string>(type: "varchar(36)", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Apelido",
                schema: "usuarios",
                table: "Usuarios",
                column: "Apelido",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                schema: "usuarios",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Usuarios",
                schema: "usuarios");
        }
    }
}
