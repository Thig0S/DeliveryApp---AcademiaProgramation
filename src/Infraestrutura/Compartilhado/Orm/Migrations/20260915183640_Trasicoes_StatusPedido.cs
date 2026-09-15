using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryApp.Infraestrutura.Compartilhado.Orm.Migrations
{
    /// <inheritdoc />
    public partial class Trasicoes_StatusPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBTransicaoStatusPedido",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PedidoId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoUsuario = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    StatusAnterior = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    StatusAtual = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Motivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    OcorridaEmUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBTransicaoStatusPedido", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBTransicaoStatusPedido_TBPedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "TBPedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBTransicaoStatusPedido_PedidoId_OcorridaEmUtc",
                table: "TBTransicaoStatusPedido",
                columns: new[] { "PedidoId", "OcorridaEmUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBTransicaoStatusPedido");
        }
    }
}
