using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    /// <inheritdoc />
    public partial class harvestMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TANK",
                columns: table => new
                {
                    TANK_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TANK", x => x.TANK_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TREATMENT",
                columns: table => new
                {
                    TREATMENT_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TYPE = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TREATMENT", x => x.TREATMENT_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WINE_BATCH",
                columns: table => new
                {
                    BATCH_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VARIETY = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AMOUNT = table.Column<double>(type: "double", nullable: false),
                    DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    MARURITY_HEALTH = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WEATHER = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WINE_BATCH", x => x.BATCH_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TANK_MOVEMENT",
                columns: table => new
                {
                    MOVEMENT_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FROM_TANK = table.Column<int>(type: "int", nullable: false),
                    TO_TANK = table.Column<int>(type: "int", nullable: false),
                    DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    VOLUME = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TANK_MOVEMENT", x => x.MOVEMENT_ID);
                    table.ForeignKey(
                        name: "FK_TANK_MOVEMENT_TANK_FROM_TANK",
                        column: x => x.FROM_TANK,
                        principalTable: "TANK",
                        principalColumn: "TANK_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TANK_MOVEMENT_TANK_TO_TANK",
                        column: x => x.TO_TANK,
                        principalTable: "TANK",
                        principalColumn: "TANK_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "INFORMATIONS",
                columns: table => new
                {
                    INFORMATION_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BATCH_ID = table.Column<int>(type: "int", nullable: false),
                    DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ACIDITY = table.Column<double>(type: "double", nullable: false),
                    PH_VALUE = table.Column<double>(type: "double", nullable: false),
                    FURTHER_STEPS = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INFORMATIONS", x => x.INFORMATION_ID);
                    table.ForeignKey(
                        name: "FK_INFORMATIONS_WINE_BATCH_BATCH_ID",
                        column: x => x.BATCH_ID,
                        principalTable: "WINE_BATCH",
                        principalColumn: "BATCH_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TANK_has_WINE_BATCH",
                columns: table => new
                {
                    TANK_ID = table.Column<int>(type: "int", nullable: false),
                    BATCH_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TANK_has_WINE_BATCH", x => new { x.BATCH_ID, x.TANK_ID });
                    table.ForeignKey(
                        name: "FK_TANK_has_WINE_BATCH_TANK_TANK_ID",
                        column: x => x.TANK_ID,
                        principalTable: "TANK",
                        principalColumn: "TANK_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TANK_has_WINE_BATCH_WINE_BATCH_BATCH_ID",
                        column: x => x.BATCH_ID,
                        principalTable: "WINE_BATCH",
                        principalColumn: "BATCH_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VINEYARD_HAS_BATCH",
                columns: table => new
                {
                    VINEYARD_ID = table.Column<int>(type: "int", nullable: false),
                    BATCH_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VINEYARD_HAS_BATCH", x => new { x.BATCH_ID, x.VINEYARD_ID });
                    table.ForeignKey(
                        name: "FK_VINEYARD_HAS_BATCH_VINEYARDS_VINEYARD_ID",
                        column: x => x.VINEYARD_ID,
                        principalTable: "VINEYARDS",
                        principalColumn: "BOOKING_OBJECT_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VINEYARD_HAS_BATCH_WINE_BATCH_BATCH_ID",
                        column: x => x.BATCH_ID,
                        principalTable: "WINE_BATCH",
                        principalColumn: "BATCH_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WINE_BATCH_has_TREATMENT",
                columns: table => new
                {
                    BATCH_ID = table.Column<int>(type: "int", nullable: false),
                    TREATMENT_ID = table.Column<int>(type: "int", nullable: false),
                    AMOUNT = table.Column<double>(type: "double", nullable: false),
                    AGENT = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WINE_BATCH_has_TREATMENT", x => new { x.BATCH_ID, x.TREATMENT_ID });
                    table.ForeignKey(
                        name: "FK_WINE_BATCH_has_TREATMENT_TREATMENT_TREATMENT_ID",
                        column: x => x.TREATMENT_ID,
                        principalTable: "TREATMENT",
                        principalColumn: "TREATMENT_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WINE_BATCH_has_TREATMENT_WINE_BATCH_BATCH_ID",
                        column: x => x.BATCH_ID,
                        principalTable: "WINE_BATCH",
                        principalColumn: "BATCH_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_INFORMATIONS_BATCH_ID",
                table: "INFORMATIONS",
                column: "BATCH_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TANK_has_WINE_BATCH_TANK_ID",
                table: "TANK_has_WINE_BATCH",
                column: "TANK_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TANK_MOVEMENT_FROM_TANK",
                table: "TANK_MOVEMENT",
                column: "FROM_TANK");

            migrationBuilder.CreateIndex(
                name: "IX_TANK_MOVEMENT_TO_TANK",
                table: "TANK_MOVEMENT",
                column: "TO_TANK");

            migrationBuilder.CreateIndex(
                name: "IX_VINEYARD_HAS_BATCH_VINEYARD_ID",
                table: "VINEYARD_HAS_BATCH",
                column: "VINEYARD_ID");

            migrationBuilder.CreateIndex(
                name: "IX_WINE_BATCH_has_TREATMENT_TREATMENT_ID",
                table: "WINE_BATCH_has_TREATMENT",
                column: "TREATMENT_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "INFORMATIONS");

            migrationBuilder.DropTable(
                name: "TANK_has_WINE_BATCH");

            migrationBuilder.DropTable(
                name: "TANK_MOVEMENT");

            migrationBuilder.DropTable(
                name: "VINEYARD_HAS_BATCH");

            migrationBuilder.DropTable(
                name: "WINE_BATCH_has_TREATMENT");

            migrationBuilder.DropTable(
                name: "TANK");

            migrationBuilder.DropTable(
                name: "TREATMENT");

            migrationBuilder.DropTable(
                name: "WINE_BATCH");
        }
    }
}
