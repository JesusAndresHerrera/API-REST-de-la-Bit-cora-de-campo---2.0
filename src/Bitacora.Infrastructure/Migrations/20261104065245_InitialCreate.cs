using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bitacora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Registros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registros", x => x.Id);
                    table.CheckConstraint("CK_Registros_Descripcion_Longitud", "length(Descripcion) BETWEEN 1 AND 500");
                    table.CheckConstraint("CK_Registros_Titulo_Longitud", "length(Titulo) BETWEEN 3 AND 100");
                });

            migrationBuilder.CreateTable(
                name: "Fotografias",
                columns: table => new
                {
                    RegistroId = table.Column<int>(type: "INTEGER", nullable: false),
                    FotoBase64 = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fotografias", x => x.RegistroId);
                    table.ForeignKey(
                        name: "FK_Fotografias_Registros_RegistroId",
                        column: x => x.RegistroId,
                        principalTable: "Registros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fotografias");

            migrationBuilder.DropTable(
                name: "Registros");
        }
    }
}
