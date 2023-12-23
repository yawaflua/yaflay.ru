using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace yaflay.ru.Migrations
{
    /// <inheritdoc />
    public partial class Migrate321220231944 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<DateTime>(
                name: "dateTime",
                schema: "public",
                table: "Blogs",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "authorId",
                schema: "public",
                table: "Blogs",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "authorNickname",
                schema: "public",
                table: "Blogs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "authorNickname",
                schema: "public",
                table: "Blogs");

            migrationBuilder.AlterColumn<DateTime>(
                name: "dateTime",
                schema: "public",
                table: "Blogs",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<int>(
                name: "authorId",
                schema: "public",
                table: "Blogs",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "Author",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    discordId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
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
    }
}
