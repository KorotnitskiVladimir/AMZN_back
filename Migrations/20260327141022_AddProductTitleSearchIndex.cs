using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMZN.Migrations
{
    /// <inheritdoc />
    public partial class AddProductTitleSearchIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Products_Title",
                table: "Products",
                column: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_Title",
                table: "Products");
        }
    }
}
