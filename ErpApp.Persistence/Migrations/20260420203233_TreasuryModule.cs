using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ErpApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TreasuryModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TreasuryAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreasuryAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TreasuryMovements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TreasuryAccountId = table.Column<int>(type: "int", nullable: false),
                    CounterpartyTreasuryAccountId = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BalanceBefore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReferenceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreasuryMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreasuryMovements_TreasuryAccounts_CounterpartyTreasuryAccountId",
                        column: x => x.CounterpartyTreasuryAccountId,
                        principalTable: "TreasuryAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TreasuryMovements_TreasuryAccounts_TreasuryAccountId",
                        column: x => x.TreasuryAccountId,
                        principalTable: "TreasuryAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TreasuryAccounts_Code",
                table: "TreasuryAccounts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreasuryMovements_CounterpartyTreasuryAccountId",
                table: "TreasuryMovements",
                column: "CounterpartyTreasuryAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TreasuryMovements_TreasuryAccountId",
                table: "TreasuryMovements",
                column: "TreasuryAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TreasuryMovements");

            migrationBuilder.DropTable(
                name: "TreasuryAccounts");
        }
    }
}
