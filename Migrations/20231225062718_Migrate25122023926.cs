using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace yaflay.ru.Migrations
{
    /// <inheritdoc />
    public partial class Migrate25122023926 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiKeys",
                schema: "public",
                columns: table => new
                {
                    Key = table.Column<string>(type: "text", nullable: false),
                    DiscordOwnerId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Melon = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.Key);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiKeys",
                schema: "public");
        }
    }
}
