using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace yaflay.ru.Migrations
{
    /// <inheritdoc />
    public partial class Migrate18122023 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "authorId",
                schema: "public",
                table: "Blogs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Author",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    discordId = table.Column<int>(type: "integer", nullable: false),
                    discordNickName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Author", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_authorId",
                schema: "public",
                table: "Blogs",
                column: "authorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Author_authorId",
                schema: "public",
                table: "Blogs",
                column: "authorId",
                principalTable: "Author",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_Author_authorId",
                schema: "public",
                table: "Blogs");

            migrationBuilder.DropTable(
                name: "Author");

            migrationBuilder.DropIndex(
                name: "IX_Blogs_authorId",
                schema: "public",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "authorId",
                schema: "public",
                table: "Blogs");
        }
    }
}
