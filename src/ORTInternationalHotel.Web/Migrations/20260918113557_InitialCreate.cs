using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ORTInternationalHotel.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hoteles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Pais = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Ciudad = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CantidadEstrellas = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hoteles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pasajeros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Documento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    PaisOrigen = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pasajeros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Habitaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numero = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Capacidad = table.Column<int>(type: "int", nullable: false),
                    PrecioPorNoche = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    HotelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Habitaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Habitaciones_Hoteles_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hoteles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaDesde = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaHasta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    MontoTotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PasajeroId = table.Column<int>(type: "int", nullable: false),
                    HotelId = table.Column<int>(type: "int", nullable: false),
                    HabitacionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservas_Habitaciones_HabitacionId",
                        column: x => x.HabitacionId,
                        principalTable: "Habitaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservas_Hoteles_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hoteles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservas_Pasajeros_PasajeroId",
                        column: x => x.PasajeroId,
                        principalTable: "Pasajeros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Hoteles",
                columns: new[] { "Id", "CantidadEstrellas", "Ciudad", "Direccion", "Nombre", "Pais" },
                values: new object[,]
                {
                    { 1, 5, "Buenos Aires", "Av. Libertador 1234", "ORT International Hotel - Buenos Aires", "Argentina" },
                    { 2, 5, "Nassau", "Ocean Drive 500", "ORT International Hotel - Bahamas", "Bahamas" },
                    { 3, 4, "Madrid", "Gran Vía 88", "ORT International Hotel - Madrid", "España" }
                });

            migrationBuilder.InsertData(
                table: "Pasajeros",
                columns: new[] { "Id", "Apellido", "Documento", "Email", "Nombre", "PaisOrigen", "Telefono" },
                values: new object[,]
                {
                    { 1, "Auday", "30111222", "karina.auday@ort.edu.ar", "Karina", "Argentina", "+54 11 4000-1111" },
                    { 2, "Smith", "US4455667", "john.smith@example.com", "John", "Estados Unidos", "+1 305-555-0110" },
                    { 3, "Fernández", "28999111", "lucia.fernandez@example.com", "Lucía", "Argentina", "+54 11 4000-2222" }
                });

            migrationBuilder.InsertData(
                table: "Habitaciones",
                columns: new[] { "Id", "Capacidad", "HotelId", "Numero", "PrecioPorNoche", "Tipo" },
                values: new object[,]
                {
                    { 1, 2, 1, "101", 120.00m, 1 },
                    { 2, 4, 1, "102", 260.00m, 2 },
                    { 3, 1, 2, "201", 90.00m, 0 },
                    { 4, 3, 2, "202", 340.00m, 2 },
                    { 5, 5, 3, "301", 210.00m, 3 }
                });

            migrationBuilder.InsertData(
                table: "Reservas",
                columns: new[] { "Id", "Estado", "FechaDesde", "FechaHasta", "HabitacionId", "HotelId", "MontoTotal", "PasajeroId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 600.00m, 1 },
                    { 2, 0, new DateTime(2026, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 12, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 2, 630.00m, 1 },
                    { 3, 1, new DateTime(2026, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 2, 1360.00m, 2 },
                    { 4, 0, new DateTime(2027, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2027, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 3, 1050.00m, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Habitaciones_HotelId",
                table: "Habitaciones",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_HabitacionId",
                table: "Reservas",
                column: "HabitacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_HotelId",
                table: "Reservas",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_PasajeroId",
                table: "Reservas",
                column: "PasajeroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reservas");

            migrationBuilder.DropTable(
                name: "Habitaciones");

            migrationBuilder.DropTable(
                name: "Pasajeros");

            migrationBuilder.DropTable(
                name: "Hoteles");
        }
    }
}
