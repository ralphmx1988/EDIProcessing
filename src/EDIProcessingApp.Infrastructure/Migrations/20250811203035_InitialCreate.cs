using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EDIProcessingApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "edi");

            migrationBuilder.CreateTable(
                name: "AccountTypes",
                schema: "edi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                schema: "edi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AccountTypeId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_AccountTypes_AccountTypeId",
                        column: x => x.AccountTypeId,
                        principalSchema: "edi",
                        principalTable: "AccountTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountConfigurations",
                schema: "edi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConfigurationKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConfigurationValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ConfigurationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "General"),
                    IsEncrypted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountConfigurations_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "edi",
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EdiFiles",
                schema: "edi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileLocation = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    FileHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EdiFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EdiFiles_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "edi",
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                schema: "edi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PartnerId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JsonData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "edi",
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Transactions_EdiFiles_FileId",
                        column: x => x.FileId,
                        principalSchema: "edi",
                        principalTable: "EdiFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "edi",
                table: "AccountTypes",
                columns: new[] { "Id", "Code", "CreatedDate", "Description", "IsActive", "Name", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, "CUST", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Customer accounts that receive goods or services", true, "Customer", null },
                    { 2, "VEND", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vendor/Supplier accounts that provide goods or services", true, "Vendor", null },
                    { 3, "PART", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Business partner accounts for collaborative transactions", true, "Partner", null },
                    { 4, "DIST", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Distributor accounts for product distribution networks", true, "Distributor", null },
                    { 5, "LOGIS", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Third-party logistics and shipping provider accounts", true, "Logistics Provider", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountConfigurations_AccountId_ConfigurationKey",
                schema: "edi",
                table: "AccountConfigurations",
                columns: new[] { "AccountId", "ConfigurationKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountTypeId",
                schema: "edi",
                table: "Accounts",
                column: "AccountTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Code",
                schema: "edi",
                table: "Accounts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountTypes_Code",
                schema: "edi",
                table: "AccountTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountTypes_IsActive",
                schema: "edi",
                table: "AccountTypes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTypes_Name",
                schema: "edi",
                table: "AccountTypes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_EdiFiles_AccountId",
                schema: "edi",
                table: "EdiFiles",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_EdiFiles_FileName",
                schema: "edi",
                table: "EdiFiles",
                column: "FileName");

            migrationBuilder.CreateIndex(
                name: "IX_EdiFiles_ReceivedAt",
                schema: "edi",
                table: "EdiFiles",
                column: "ReceivedAt");

            migrationBuilder.CreateIndex(
                name: "IX_EdiFiles_Status",
                schema: "edi",
                table: "EdiFiles",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountId",
                schema: "edi",
                table: "Transactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_FileId",
                schema: "edi",
                table: "Transactions",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PartnerId",
                schema: "edi",
                table: "Transactions",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ProcessedAt",
                schema: "edi",
                table: "Transactions",
                column: "ProcessedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Status",
                schema: "edi",
                table: "Transactions",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountConfigurations",
                schema: "edi");

            migrationBuilder.DropTable(
                name: "Transactions",
                schema: "edi");

            migrationBuilder.DropTable(
                name: "EdiFiles",
                schema: "edi");

            migrationBuilder.DropTable(
                name: "Accounts",
                schema: "edi");

            migrationBuilder.DropTable(
                name: "AccountTypes",
                schema: "edi");
        }
    }
}
