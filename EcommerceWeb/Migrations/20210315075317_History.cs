using Microsoft.EntityFrameworkCore.Migrations;

namespace EcommerceWebApi.Migrations
{
    public partial class History : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductImagePath",
                table: "OrderDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "OrderDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ProductPrice",
                table: "OrderDetail",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "VariantType",
                table: "OrderDetail",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductImagePath",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "ProductPrice",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "VariantType",
                table: "OrderDetail");
        }
    }
}
