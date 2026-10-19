using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learniverse.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEnrollmentProvisioningStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EnrollmentProvisioningStatus",
                table: "Payments",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "NotStarted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnrollmentProvisioningStatus",
                table: "Payments");
        }
    }
}
