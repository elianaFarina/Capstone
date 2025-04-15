using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MuseoMineralogia.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TipiBiglietto",
                columns: table => new
                {
                    TipoBigliettoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Prezzo = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipiBiglietto", x => x.TipoBigliettoId);
                });

            migrationBuilder.CreateTable(
                name: "Utenti",
                columns: table => new
                {
                    UtenteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Cognome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utenti", x => x.UtenteId);
                });

            migrationBuilder.CreateTable(
                name: "Carrelli",
                columns: table => new
                {
                    CarrelloId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtenteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carrelli", x => x.CarrelloId);
                    table.ForeignKey(
                        name: "FK_Carrelli_Utenti_UtenteId",
                        column: x => x.UtenteId,
                        principalTable: "Utenti",
                        principalColumn: "UtenteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ordini",
                columns: table => new
                {
                    OrdineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtenteId = table.Column<int>(type: "int", nullable: false),
                    DataOrdine = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Stato = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ordini", x => x.OrdineId);
                    table.ForeignKey(
                        name: "FK_Ordini_Utenti_UtenteId",
                        column: x => x.UtenteId,
                        principalTable: "Utenti",
                        principalColumn: "UtenteId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ElementiCarrello",
                columns: table => new
                {
                    ElementoCarrelloId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarrelloId = table.Column<int>(type: "int", nullable: false),
                    TipoBigliettoId = table.Column<int>(type: "int", nullable: false),
                    Quantita = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementiCarrello", x => x.ElementoCarrelloId);
                    table.ForeignKey(
                        name: "FK_ElementiCarrello_Carrelli_CarrelloId",
                        column: x => x.CarrelloId,
                        principalTable: "Carrelli",
                        principalColumn: "CarrelloId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ElementiCarrello_TipiBiglietto_TipoBigliettoId",
                        column: x => x.TipoBigliettoId,
                        principalTable: "TipiBiglietto",
                        principalColumn: "TipoBigliettoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DettagliOrdine",
                columns: table => new
                {
                    DettaglioOrdineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdineId = table.Column<int>(type: "int", nullable: false),
                    TipoBigliettoId = table.Column<int>(type: "int", nullable: false),
                    Quantita = table.Column<int>(type: "int", nullable: false),
                    PrezzoUnitario = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DettagliOrdine", x => x.DettaglioOrdineId);
                    table.ForeignKey(
                        name: "FK_DettagliOrdine_Ordini_OrdineId",
                        column: x => x.OrdineId,
                        principalTable: "Ordini",
                        principalColumn: "OrdineId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DettagliOrdine_TipiBiglietto_TipoBigliettoId",
                        column: x => x.TipoBigliettoId,
                        principalTable: "TipiBiglietto",
                        principalColumn: "TipoBigliettoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "TipiBiglietto",
                columns: new[] { "TipoBigliettoId", "Nome", "Prezzo" },
                values: new object[,]
                {
                    { 1, "Intero", 10.00m },
                    { 2, "Ridotto (fino a 15 anni)", 8.00m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Carrelli_UtenteId",
                table: "Carrelli",
                column: "UtenteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DettagliOrdine_OrdineId",
                table: "DettagliOrdine",
                column: "OrdineId");

            migrationBuilder.CreateIndex(
                name: "IX_DettagliOrdine_TipoBigliettoId",
                table: "DettagliOrdine",
                column: "TipoBigliettoId");

            migrationBuilder.CreateIndex(
                name: "IX_ElementiCarrello_CarrelloId",
                table: "ElementiCarrello",
                column: "CarrelloId");

            migrationBuilder.CreateIndex(
                name: "IX_ElementiCarrello_TipoBigliettoId",
                table: "ElementiCarrello",
                column: "TipoBigliettoId");

            migrationBuilder.CreateIndex(
                name: "IX_Ordini_UtenteId",
                table: "Ordini",
                column: "UtenteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DettagliOrdine");

            migrationBuilder.DropTable(
                name: "ElementiCarrello");

            migrationBuilder.DropTable(
                name: "Ordini");

            migrationBuilder.DropTable(
                name: "Carrelli");

            migrationBuilder.DropTable(
                name: "TipiBiglietto");

            migrationBuilder.DropTable(
                name: "Utenti");
        }
    }
}
