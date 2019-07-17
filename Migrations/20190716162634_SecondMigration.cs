using Microsoft.EntityFrameworkCore.Migrations;

namespace WeddingPlanner.Migrations
{
    public partial class SecondMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegUserId",
                table: "Weddings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Weddings_RegUserId",
                table: "Weddings",
                column: "RegUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Weddings_Users_RegUserId",
                table: "Weddings",
                column: "RegUserId",
                principalTable: "Users",
                principalColumn: "RegUserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Weddings_Users_RegUserId",
                table: "Weddings");

            migrationBuilder.DropIndex(
                name: "IX_Weddings_RegUserId",
                table: "Weddings");

            migrationBuilder.DropColumn(
                name: "RegUserId",
                table: "Weddings");
        }
    }
}
