using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillPaymentApi.Migrations
{
    /// <inheritdoc />
    public partial class AddLoggingDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAuthenticated",
                table: "ApiLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RequestHeaders",
                table: "ApiLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RequestSize",
                table: "ApiLogs",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ResponseSize",
                table: "ApiLogs",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAuthenticated",
                table: "ApiLogs");

            migrationBuilder.DropColumn(
                name: "RequestHeaders",
                table: "ApiLogs");

            migrationBuilder.DropColumn(
                name: "RequestSize",
                table: "ApiLogs");

            migrationBuilder.DropColumn(
                name: "ResponseSize",
                table: "ApiLogs");
        }
    }
}
