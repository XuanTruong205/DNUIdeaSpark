using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNU.IdeaSpark.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdeaNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ideas_Users_SubmitterUserId",
                table: "Ideas");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Ideas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Ideas_Users_SubmitterUserId",
                table: "Ideas",
                column: "SubmitterUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ideas_Users_SubmitterUserId",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Ideas");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Ideas_Users_SubmitterUserId",
                table: "Ideas",
                column: "SubmitterUserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
