using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ErpApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class WarehouseOnPurchasesAndSales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "PurchaseOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "Invoices",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
DECLARE @defaultWarehouseId INT;

SELECT TOP(1) @defaultWarehouseId = [Id]
FROM [Warehouses]
ORDER BY [Id];

IF @defaultWarehouseId IS NULL
BEGIN
    INSERT INTO [Warehouses] ([Code], [Name], [IsActive])
    VALUES ('MAIN', 'Almacén Principal', 1);

    SET @defaultWarehouseId = CAST(SCOPE_IDENTITY() AS INT);
END

UPDATE [PurchaseOrders]
SET [WarehouseId] = @defaultWarehouseId
WHERE [WarehouseId] = 0;
");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_WarehouseId",
                table: "PurchaseOrders",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_WarehouseId",
                table: "Invoices",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Warehouses_WarehouseId",
                table: "Invoices",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Warehouses_WarehouseId",
                table: "PurchaseOrders",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Warehouses_WarehouseId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Warehouses_WarehouseId",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_WarehouseId",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_WarehouseId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "Invoices");
        }
    }
}
