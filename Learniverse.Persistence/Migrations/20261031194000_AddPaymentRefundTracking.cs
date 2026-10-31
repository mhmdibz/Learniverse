using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learniverse.Persistence.Migrations;

public partial class AddPaymentRefundTracking : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "RefundStatus",
            table: "Payments",
            type: "nvarchar(20)",
            maxLength: 20,
            nullable: false,
            defaultValue: "NotRequested");

        migrationBuilder.AddColumn<DateTime>(
            name: "RefundRequestedAtUtc",
            table: "Payments",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<byte[]>(
            name: "RowVersion",
            table: "Payments",
            type: "rowversion",
            rowVersion: true,
            nullable: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "RefundStatus",
            table: "Payments");

        migrationBuilder.DropColumn(
            name: "RefundRequestedAtUtc",
            table: "Payments");

        migrationBuilder.DropColumn(
            name: "RowVersion",
            table: "Payments");
    }
}
