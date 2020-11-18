using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Softplan.Justica.GerenciadorProcessos.Infra.DataAccess.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Responsavel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(maxLength: 150, nullable: false),
                    Cpf = table.Column<string>(maxLength: 11, nullable: false),
                    Email = table.Column<string>(maxLength: 400, nullable: false),
                    Foto = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Responsavel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SituacaoProcesso",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(nullable: false),
                    Finalizado = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SituacaoProcesso", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Processo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NumeroProcesso = table.Column<string>(maxLength: 20, nullable: false),
                    DataDistribuicao = table.Column<DateTimeOffset>(nullable: true),
                    SegredoJustica = table.Column<bool>(nullable: false),
                    PastaFisicaCliente = table.Column<string>(maxLength: 50, nullable: true),
                    Descricao = table.Column<string>(maxLength: 1000, nullable: true),
                    ProcessoVinculadoId = table.Column<int>(nullable: true),
                    SituacaoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Processo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Processo_Processo_ProcessoVinculadoId",
                        column: x => x.ProcessoVinculadoId,
                        principalTable: "Processo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Processo_SituacaoProcesso_SituacaoId",
                        column: x => x.SituacaoId,
                        principalTable: "SituacaoProcesso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessoResponsavel",
                columns: table => new
                {
                    ProcessoId = table.Column<int>(nullable: false),
                    ResponsavelId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessoResponsavel", x => new { x.ProcessoId, x.ResponsavelId });
                    table.ForeignKey(
                        name: "FK_ProcessoResponsavel_Processo_ProcessoId",
                        column: x => x.ProcessoId,
                        principalTable: "Processo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcessoResponsavel_Responsavel_ResponsavelId",
                        column: x => x.ResponsavelId,
                        principalTable: "Responsavel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "SituacaoProcesso",
                columns: new[] { "Id", "Finalizado", "Nome" },
                values: new object[,]
                {
                    { 1, false, "Em Andamento" },
                    { 2, false, "Desmembrado" },
                    { 3, false, "Em Recurso" },
                    { 4, true, "Finalizado" },
                    { 5, true, "Arquivado" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Processo_NumeroProcesso",
                table: "Processo",
                column: "NumeroProcesso",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Processo_ProcessoVinculadoId",
                table: "Processo",
                column: "ProcessoVinculadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Processo_SituacaoId",
                table: "Processo",
                column: "SituacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessoResponsavel_ResponsavelId",
                table: "ProcessoResponsavel",
                column: "ResponsavelId");

            migrationBuilder.CreateIndex(
                name: "IX_Responsavel_Cpf",
                table: "Responsavel",
                column: "Cpf",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessoResponsavel");

            migrationBuilder.DropTable(
                name: "Processo");

            migrationBuilder.DropTable(
                name: "Responsavel");

            migrationBuilder.DropTable(
                name: "SituacaoProcesso");
        }
    }
}
