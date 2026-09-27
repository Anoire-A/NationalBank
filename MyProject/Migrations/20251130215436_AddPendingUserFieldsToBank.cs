using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migrations
{
    /// <inheritdoc />
    public partial class AddPendingUserFieldsToBank : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BankId",
                table: "Banks",
                newName: "Id");

            migrationBuilder.AlterColumn<bool>(
                name: "IsApproved",
                table: "Banks",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "AssociatedUserId",
                table: "Banks",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PendingPasswordHash",
                table: "Banks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PendingUserEmail",
                table: "Banks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Banks_AssociatedUserId",
                table: "Banks",
                column: "AssociatedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Banks_AspNetUsers_AssociatedUserId",
                table: "Banks",
                column: "AssociatedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Banks_AspNetUsers_AssociatedUserId",
                table: "Banks");

            migrationBuilder.DropIndex(
                name: "IX_Banks_AssociatedUserId",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "PendingPasswordHash",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "PendingUserEmail",
                table: "Banks");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Banks",
                newName: "BankId");

            migrationBuilder.AlterColumn<int>(
                name: "IsApproved",
                table: "Banks",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "AssociatedUserId",
                table: "Banks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
