using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace KozossegAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Ceg",
                columns: table => new
                {
                    ceg_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    cegnev = table.Column<string>(type: "longtext", nullable: false),
                    ceg_email = table.Column<string>(type: "longtext", nullable: false),
                    cim = table.Column<string>(type: "longtext", nullable: false),
                    adoszam = table.Column<string>(type: "longtext", nullable: false),
                    jelszo = table.Column<string>(type: "longtext", nullable: false),
                    telefonszam = table.Column<string>(type: "longtext", nullable: false),
                    is_active = table.Column<int>(type: "int", nullable: false),
                    verification_token = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ceg", x => x.ceg_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Diak",
                columns: table => new
                {
                    diak_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    nev = table.Column<string>(type: "longtext", nullable: false),
                    email = table.Column<string>(type: "longtext", nullable: false),
                    jelszo = table.Column<string>(type: "longtext", nullable: false),
                    iskola = table.Column<string>(type: "longtext", nullable: true),
                    telefonszam = table.Column<string>(type: "longtext", nullable: true),
                    igazolas_pdf = table.Column<string>(type: "longtext", nullable: true),
                    ledolgozott_ora = table.Column<int>(type: "int", nullable: false),
                    om_azonosito = table.Column<string>(type: "longtext", nullable: true),
                    iskola_om_kod = table.Column<string>(type: "longtext", nullable: true),
                    osztaly = table.Column<string>(type: "longtext", nullable: true),
                    is_active = table.Column<int>(type: "int", nullable: false),
                    verification_token = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diak", x => x.diak_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Dokumentum",
                columns: table => new
                {
                    doku_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ceg_id = table.Column<int>(type: "int", nullable: false),
                    pdf_link = table.Column<string>(type: "longtext", nullable: false),
                    alairas_datuma = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dokumentum", x => x.doku_id);
                    table.ForeignKey(
                        name: "FK_Dokumentum_Ceg_ceg_id",
                        column: x => x.ceg_id,
                        principalTable: "Ceg",
                        principalColumn: "ceg_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Munka",
                columns: table => new
                {
                    munka_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ceg_id = table.Column<int>(type: "int", nullable: false),
                    munka_nev = table.Column<string>(type: "longtext", nullable: false),
                    cim = table.Column<string>(type: "longtext", nullable: false),
                    idopont = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    oraszam = table.Column<int>(type: "int", nullable: false),
                    letszam = table.Column<int>(type: "int", nullable: false),
                    leiras = table.Column<string>(type: "longtext", nullable: false),
                    statusz = table.Column<string>(type: "longtext", nullable: false),
                    kategoria = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Munka", x => x.munka_id);
                    table.ForeignKey(
                        name: "FK_Munka_Ceg_ceg_id",
                        column: x => x.ceg_id,
                        principalTable: "Ceg",
                        principalColumn: "ceg_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Jelentkezes",
                columns: table => new
                {
                    munka_id = table.Column<int>(type: "int", nullable: false),
                    diak_id = table.Column<int>(type: "int", nullable: false),
                    jelentkezes_ideje = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    munka_statusz = table.Column<string>(type: "longtext", nullable: false),
                    jovahagyott_ora = table.Column<int>(type: "int", nullable: false),
                    igazolas_adatok = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jelentkezes", x => new { x.munka_id, x.diak_id });
                    table.ForeignKey(
                        name: "FK_Jelentkezes_Diak_diak_id",
                        column: x => x.diak_id,
                        principalTable: "Diak",
                        principalColumn: "diak_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Jelentkezes_Munka_munka_id",
                        column: x => x.munka_id,
                        principalTable: "Munka",
                        principalColumn: "munka_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Dokumentum_ceg_id",
                table: "Dokumentum",
                column: "ceg_id");

            migrationBuilder.CreateIndex(
                name: "IX_Jelentkezes_diak_id",
                table: "Jelentkezes",
                column: "diak_id");

            migrationBuilder.CreateIndex(
                name: "IX_Munka_ceg_id",
                table: "Munka",
                column: "ceg_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dokumentum");

            migrationBuilder.DropTable(
                name: "Jelentkezes");

            migrationBuilder.DropTable(
                name: "Diak");

            migrationBuilder.DropTable(
                name: "Munka");

            migrationBuilder.DropTable(
                name: "Ceg");
        }
    }
}
