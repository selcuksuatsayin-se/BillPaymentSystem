using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillPaymentApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBalanceColumnAndRemoveNameColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Subscribers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Subscribers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "Subscribers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Subscribers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
