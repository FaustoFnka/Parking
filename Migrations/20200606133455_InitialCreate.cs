using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Parking.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PriceTable",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InicioVigencia = table.Column<DateTime>(nullable: false),
                    FimVigencia = table.Column<DateTime>(nullable: false),
                    ValorHoraInical = table.Column<float>(nullable: false),
                    ValorHoraAdicional = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleControl",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Placa = table.Column<string>(nullable: false),
                    HoraEntrada = table.Column<DateTime>(nullable: false),
                    HoraSaida = table.Column<DateTime>(nullable: false),
                    Duracao = table.Column<DateTime>(nullable: true),
                    QtdHorasCobradas = table.Column<int>(nullable: true),
                    ValorHora = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleControl", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceTable");

            migrationBuilder.DropTable(
                name: "VehicleControl");
        }
    }
}
