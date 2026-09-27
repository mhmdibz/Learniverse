using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learniverse.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class VerifyFilteredIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sections_CourseId_Order",
                table: "Sections");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_SectionId_Order",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Name",
                table: "Categories");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_CourseId_Order",
                table: "Sections",
                columns: new[] { "CourseId", "Order" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_SectionId_Order",
                table: "Lessons",
                columns: new[] { "SectionId", "Order" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sections_CourseId_Order",
                table: "Sections");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_SectionId_Order",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Name",
                table: "Categories");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_CourseId_Order",
                table: "Sections",
                columns: new[] { "CourseId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_SectionId_Order",
                table: "Lessons",
                columns: new[] { "SectionId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);
        }
    }
}
