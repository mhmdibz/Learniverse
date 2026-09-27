using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learniverse.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sections_CourseId",
                table: "Sections");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_CourseId_Order",
                table: "Sections",
                columns: new[] { "CourseId", "Order" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sections_CourseId_Order",
                table: "Sections");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_CourseId",
                table: "Sections",
                column: "CourseId");
        }
    }
}
