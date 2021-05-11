using Microsoft.EntityFrameworkCore.Migrations;

namespace EcommerceWebApi.Migrations
{
    public partial class MemberInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Test",
                table: "Product");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Test",
                table: "Product",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
