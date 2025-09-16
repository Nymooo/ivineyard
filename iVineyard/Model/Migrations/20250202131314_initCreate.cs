using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    /// <inheritdoc />
    public partial class initCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BOOKING_OBJECTS_BT",
                columns: table => new
                {
                    BOOKING_OBJECT_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BOOKING_OBJECTS_BT", x => x.BOOKING_OBJECT_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "COMPANIES",
                columns: table => new
                {
                    COMPANY_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NAME = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COMPANIES", x => x.COMPANY_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "E_MACHINE_STATUS_TYPES",
                columns: table => new
                {
                    STATUS_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TYPE = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_E_MACHINE_STATUS_TYPES", x => x.STATUS_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "E_VINEYARD_STATUS_TYPE",
                columns: table => new
                {
                    STATUS_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TYPE = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_E_VINEYARD_STATUS_TYPE", x => x.STATUS_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EQUIPMENTS",
                columns: table => new
                {
                    BOOKING_OBJECT_ID = table.Column<int>(type: "int", nullable: false),
                    NAME = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EQUIPMENTS", x => x.BOOKING_OBJECT_ID);
                    table.ForeignKey(
                        name: "FK_EQUIPMENTS_BOOKING_OBJECTS_BT_BOOKING_OBJECT_ID",
                        column: x => x.BOOKING_OBJECT_ID,
                        principalTable: "BOOKING_OBJECTS_BT",
                        principalColumn: "BOOKING_OBJECT_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "INVOICES",
                columns: table => new
                {
                    INVOICE_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BOOKING_OBJECT_ID = table.Column<int>(type: "int", nullable: true),
                    PRICE = table.Column<double>(type: "double", nullable: false),
                    BOUGHT_AT = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DESCRIPTION = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INVOICES", x => x.INVOICE_ID);
                    table.ForeignKey(
                        name: "FK_INVOICES_BOOKING_OBJECTS_BT_BOOKING_OBJECT_ID",
                        column: x => x.BOOKING_OBJECT_ID,
                        principalTable: "BOOKING_OBJECTS_BT",
                        principalColumn: "BOOKING_OBJECT_ID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MACHINES",
                columns: table => new
                {
                    BOOKING_OBJECT_ID = table.Column<int>(type: "int", nullable: false),
                    NAME = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MACHINES", x => x.BOOKING_OBJECT_ID);
                    table.ForeignKey(
                        name: "FK_MACHINES_BOOKING_OBJECTS_BT_BOOKING_OBJECT_ID",
                        column: x => x.BOOKING_OBJECT_ID,
                        principalTable: "BOOKING_OBJECTS_BT",
                        principalColumn: "BOOKING_OBJECT_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BOOKING_OBJECT_ID = table.Column<int>(type: "int", nullable: false),
                    COMPANY_ID = table.Column<int>(type: "int", nullable: true),
                    SALARY = table.Column<double>(type: "double", nullable: false),
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_BOOKING_OBJECTS_BT_BOOKING_OBJECT_ID",
                        column: x => x.BOOKING_OBJECT_ID,
                        principalTable: "BOOKING_OBJECTS_BT",
                        principalColumn: "BOOKING_OBJECT_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_COMPANIES_COMPANY_ID",
                        column: x => x.COMPANY_ID,
                        principalTable: "COMPANIES",
                        principalColumn: "COMPANY_ID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VINEYARDS",
                columns: table => new
                {
                    BOOKING_OBJECT_ID = table.Column<int>(type: "int", nullable: false),
                    COMPANY_ID = table.Column<int>(type: "int", nullable: true),
                    NAME = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    COORDINATES = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MID_COORDINATE = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AREA = table.Column<float>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VINEYARDS", x => x.BOOKING_OBJECT_ID);
                    table.ForeignKey(
                        name: "FK_VINEYARDS_BOOKING_OBJECTS_BT_BOOKING_OBJECT_ID",
                        column: x => x.BOOKING_OBJECT_ID,
                        principalTable: "BOOKING_OBJECTS_BT",
                        principalColumn: "BOOKING_OBJECT_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VINEYARDS_COMPANIES_COMPANY_ID",
                        column: x => x.COMPANY_ID,
                        principalTable: "COMPANIES",
                        principalColumn: "COMPANY_ID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MACHINE_HAS_STATUS",
                columns: table => new
                {
                    MACHINE_ID = table.Column<int>(type: "int", nullable: false),
                    STATUS_ID = table.Column<int>(type: "int", nullable: false),
                    START_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    END_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MACHINE_HAS_STATUS", x => new { x.MACHINE_ID, x.STATUS_ID, x.START_DATE });
                    table.ForeignKey(
                        name: "FK_MACHINE_HAS_STATUS_E_MACHINE_STATUS_TYPES_STATUS_ID",
                        column: x => x.STATUS_ID,
                        principalTable: "E_MACHINE_STATUS_TYPES",
                        principalColumn: "STATUS_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MACHINE_HAS_STATUS_MACHINES_MACHINE_ID",
                        column: x => x.MACHINE_ID,
                        principalTable: "MACHINES",
                        principalColumn: "BOOKING_OBJECT_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderKey = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VINEYARD_HAS_STATUS_JT",
                columns: table => new
                {
                    BOOKING_OBJECT_ID = table.Column<int>(type: "int", nullable: false),
                    STATUS_ID = table.Column<int>(type: "int", nullable: false),
                    START_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    END_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VINEYARD_HAS_STATUS_JT", x => new { x.BOOKING_OBJECT_ID, x.STATUS_ID, x.START_DATE });
                    table.ForeignKey(
                        name: "FK_VINEYARD_HAS_STATUS_JT_E_VINEYARD_STATUS_TYPE_STATUS_ID",
                        column: x => x.STATUS_ID,
                        principalTable: "E_VINEYARD_STATUS_TYPE",
                        principalColumn: "STATUS_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VINEYARD_HAS_STATUS_JT_VINEYARDS_BOOKING_OBJECT_ID",
                        column: x => x.BOOKING_OBJECT_ID,
                        principalTable: "VINEYARDS",
                        principalColumn: "BOOKING_OBJECT_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VINEYARD_WORK_INFORMATION_JT",
                columns: table => new
                {
                    WORK_INFORMATION_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    USER_ID = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BOOKING_OBJECT_ID = table.Column<int>(type: "int", nullable: false),
                    STARTED_AT = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    MACHINE_ID = table.Column<int>(type: "int", nullable: true),
                    FINISHED_AT = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DESCRIPTION = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VINEYARD_WORK_INFORMATION_JT", x => new { x.WORK_INFORMATION_ID, x.BOOKING_OBJECT_ID, x.USER_ID, x.STARTED_AT });
                    table.ForeignKey(
                        name: "FK_VINEYARD_WORK_INFORMATION_JT_AspNetUsers_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VINEYARD_WORK_INFORMATION_JT_MACHINES_MACHINE_ID",
                        column: x => x.MACHINE_ID,
                        principalTable: "MACHINES",
                        principalColumn: "BOOKING_OBJECT_ID");
                    table.ForeignKey(
                        name: "FK_VINEYARD_WORK_INFORMATION_JT_VINEYARDS_BOOKING_OBJECT_ID",
                        column: x => x.BOOKING_OBJECT_ID,
                        principalTable: "VINEYARDS",
                        principalColumn: "BOOKING_OBJECT_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_BOOKING_OBJECT_ID",
                table: "AspNetUsers",
                column: "BOOKING_OBJECT_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_COMPANY_ID",
                table: "AspNetUsers",
                column: "COMPANY_ID");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_COMPANIES_NAME",
                table: "COMPANIES",
                column: "NAME",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_INVOICES_BOOKING_OBJECT_ID",
                table: "INVOICES",
                column: "BOOKING_OBJECT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_MACHINE_HAS_STATUS_STATUS_ID",
                table: "MACHINE_HAS_STATUS",
                column: "STATUS_ID");

            migrationBuilder.CreateIndex(
                name: "IX_VINEYARD_HAS_STATUS_JT_STATUS_ID",
                table: "VINEYARD_HAS_STATUS_JT",
                column: "STATUS_ID");

            migrationBuilder.CreateIndex(
                name: "IX_VINEYARD_WORK_INFORMATION_JT_BOOKING_OBJECT_ID",
                table: "VINEYARD_WORK_INFORMATION_JT",
                column: "BOOKING_OBJECT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_VINEYARD_WORK_INFORMATION_JT_MACHINE_ID",
                table: "VINEYARD_WORK_INFORMATION_JT",
                column: "MACHINE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_VINEYARD_WORK_INFORMATION_JT_USER_ID",
                table: "VINEYARD_WORK_INFORMATION_JT",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_VINEYARDS_COMPANY_ID",
                table: "VINEYARDS",
                column: "COMPANY_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "EQUIPMENTS");

            migrationBuilder.DropTable(
                name: "INVOICES");

            migrationBuilder.DropTable(
                name: "MACHINE_HAS_STATUS");

            migrationBuilder.DropTable(
                name: "VINEYARD_HAS_STATUS_JT");

            migrationBuilder.DropTable(
                name: "VINEYARD_WORK_INFORMATION_JT");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "E_MACHINE_STATUS_TYPES");

            migrationBuilder.DropTable(
                name: "E_VINEYARD_STATUS_TYPE");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "MACHINES");

            migrationBuilder.DropTable(
                name: "VINEYARDS");

            migrationBuilder.DropTable(
                name: "BOOKING_OBJECTS_BT");

            migrationBuilder.DropTable(
                name: "COMPANIES");
        }
    }
}
