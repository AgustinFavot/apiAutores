using Microsoft.EntityFrameworkCore.Migrations;

namespace apiVS.Migrations
{
    public partial class comments_users : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "Comments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UsuarioId",
                table: "Comments",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_UsuarioId",
                table: "Comments",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_UsuarioId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_UsuarioId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Comments");
        }
    }
}
