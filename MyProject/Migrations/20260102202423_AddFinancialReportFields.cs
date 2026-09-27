using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migrations
{
    /// <inheritdoc />
    public partial class AddFinancialReportFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinancialReports_Banks_BankId1",
                table: "FinancialReports");

            migrationBuilder.DropIndex(
                name: "IX_FinancialReports_BankId1",
                table: "FinancialReports");

            migrationBuilder.RenameColumn(
                name: "BankId1",
                table: "FinancialReports",
                newName: "Status");

            migrationBuilder.AlterColumn<string>(
                name: "SubmittedByUserId",
                table: "FinancialReports",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "BankId",
                table: "FinancialReports",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<decimal>(
                name: "NetProfit",
                table: "FinancialReports",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NonPerformingLoans",
                table: "FinancialReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ReviewNotes",
                table: "FinancialReports",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "FinancialReports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewedByUserId",
                table: "FinancialReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinancialReports_BankId",
                table: "FinancialReports",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialReports_SubmittedByUserId",
                table: "FinancialReports",
                column: "SubmittedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialReports_AspNetUsers_SubmittedByUserId",
                table: "FinancialReports",
                column: "SubmittedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialReports_Banks_BankId",
                table: "FinancialReports",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinancialReports_AspNetUsers_SubmittedByUserId",
                table: "FinancialReports");

            migrationBuilder.DropForeignKey(
                name: "FK_FinancialReports_Banks_BankId",
                table: "FinancialReports");

            migrationBuilder.DropIndex(
                name: "IX_FinancialReports_BankId",
                table: "FinancialReports");

            migrationBuilder.DropIndex(
                name: "IX_FinancialReports_SubmittedByUserId",
                table: "FinancialReports");

            migrationBuilder.DropColumn(
                name: "NetProfit",
                table: "FinancialReports");

            migrationBuilder.DropColumn(
                name: "NonPerformingLoans",
                table: "FinancialReports");

            migrationBuilder.DropColumn(
                name: "ReviewNotes",
                table: "FinancialReports");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "FinancialReports");

            migrationBuilder.DropColumn(
                name: "ReviewedByUserId",
                table: "FinancialReports");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "FinancialReports",
                newName: "BankId1");

            migrationBuilder.AlterColumn<string>(
                name: "SubmittedByUserId",
                table: "FinancialReports",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "BankId",
                table: "FinancialReports",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialReports_BankId1",
                table: "FinancialReports",
                column: "BankId1");

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialReports_Banks_BankId1",
                table: "FinancialReports",
                column: "BankId1",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
