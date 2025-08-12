using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EDIProcessingApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEdiTransactionTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EdiTransactionTypeId",
                schema: "edi",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EdiTransactionTypeId",
                schema: "edi",
                table: "EdiFiles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EdiTransactionTypes",
                schema: "edi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    X12Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DocumentName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EdifactName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Direction = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Both"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EdiTransactionTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "edi",
                table: "EdiTransactionTypes",
                columns: new[] { "Id", "CreatedDate", "Description", "Direction", "DocumentName", "EdifactName", "IsActive", "UpdatedDate", "X12Code" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Order request", "Both", "Purchase Order", "ORDERS", true, null, "850" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Billing information", "Both", "Invoice", "INVOIC", true, null, "810" },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shipping notification", "Inbound", "Advance Ship Notice", "DESADV", true, null, "856" },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "PO confirmation", "Outbound", "Purchase Order Acknowledgement", "ORDRSP", true, null, "855" },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Payment/remittance information", "Both", "Payment Order/Remittance", "PAYMUL", true, null, "820" },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Delivery schedule", "Both", "Shipping Schedule", "DELFOR", true, null, "862" },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Receipt confirmation of EDI message", "Both", "Functional Acknowledgment", "CONTRL", true, null, "997" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_EdiTransactionTypeId",
                schema: "edi",
                table: "Transactions",
                column: "EdiTransactionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EdiFiles_EdiTransactionTypeId",
                schema: "edi",
                table: "EdiFiles",
                column: "EdiTransactionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EdiTransactionTypes_DocumentName",
                schema: "edi",
                table: "EdiTransactionTypes",
                column: "DocumentName");

            migrationBuilder.CreateIndex(
                name: "IX_EdiTransactionTypes_EdifactName",
                schema: "edi",
                table: "EdiTransactionTypes",
                column: "EdifactName");

            migrationBuilder.CreateIndex(
                name: "IX_EdiTransactionTypes_IsActive",
                schema: "edi",
                table: "EdiTransactionTypes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_EdiTransactionTypes_X12Code",
                schema: "edi",
                table: "EdiTransactionTypes",
                column: "X12Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EdiFiles_EdiTransactionTypes_EdiTransactionTypeId",
                schema: "edi",
                table: "EdiFiles",
                column: "EdiTransactionTypeId",
                principalSchema: "edi",
                principalTable: "EdiTransactionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_EdiTransactionTypes_EdiTransactionTypeId",
                schema: "edi",
                table: "Transactions",
                column: "EdiTransactionTypeId",
                principalSchema: "edi",
                principalTable: "EdiTransactionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EdiFiles_EdiTransactionTypes_EdiTransactionTypeId",
                schema: "edi",
                table: "EdiFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_EdiTransactionTypes_EdiTransactionTypeId",
                schema: "edi",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "EdiTransactionTypes",
                schema: "edi");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_EdiTransactionTypeId",
                schema: "edi",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_EdiFiles_EdiTransactionTypeId",
                schema: "edi",
                table: "EdiFiles");

            migrationBuilder.DropColumn(
                name: "EdiTransactionTypeId",
                schema: "edi",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "EdiTransactionTypeId",
                schema: "edi",
                table: "EdiFiles");
        }
    }
}
