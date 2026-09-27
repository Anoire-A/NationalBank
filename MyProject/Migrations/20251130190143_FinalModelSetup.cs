using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migrations
{
    /// <inheritdoc />
    public partial class FinalModelSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Banks_BankId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_FinancialReports_Banks_BankId",
                table: "FinancialReports");

            migrationBuilder.DropIndex(
                name: "IX_FinancialReports_BankId",
                table: "FinancialReports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Banks",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Banks");

            migrationBuilder.AlterColumn<string>(
                name: "BankId",
                table: "FinancialReports",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "BankId1",
                table: "FinancialReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "RegistrationNumber",
                table: "Banks",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Banks",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "BankId",
                table: "Banks",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Banks",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BankId",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Banks",
                table: "Banks",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialReports_BankId1",
                table: "FinancialReports",
                column: "BankId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Banks_BankId",
                table: "AspNetUsers",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "BankId");

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialReports_Banks_BankId1",
                table: "FinancialReports",
                column: "BankId1",
                principalTable: "Banks",
                principalColumn: "BankId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Banks_BankId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_FinancialReports_Banks_BankId1",
                table: "FinancialReports");

            migrationBuilder.DropIndex(
                name: "IX_FinancialReports_BankId1",
                table: "FinancialReports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Banks",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "BankId1",
                table: "FinancialReports");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Banks");

            migrationBuilder.AlterColumn<string>(
                name: "BankId",
                table: "FinancialReports",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "RegistrationNumber",
                table: "Banks",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Banks",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Banks",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "BankId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Banks",
                table: "Banks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialReports_BankId",
                table: "FinancialReports",
                column: "BankId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Banks_BankId",
                table: "AspNetUsers",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialReports_Banks_BankId",
                table: "FinancialReports",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
