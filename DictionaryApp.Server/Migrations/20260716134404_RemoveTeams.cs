using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DictionaryApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DictionaryEntries_Teams_TeamId",
                table: "DictionaryEntries");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_DictionaryEntries_TeamId_Key",
                table: "DictionaryEntries");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "DictionaryEntries");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "DictionaryEntries",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryEntries_UserId_Key",
                table: "DictionaryEntries",
                columns: new[] { "UserId", "Key" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DictionaryEntries_UserId_Key",
                table: "DictionaryEntries");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DictionaryEntries");

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "DictionaryEntries",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryEntries_TeamId_Key",
                table: "DictionaryEntries",
                columns: new[] { "TeamId", "Key" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DictionaryEntries_Teams_TeamId",
                table: "DictionaryEntries",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
