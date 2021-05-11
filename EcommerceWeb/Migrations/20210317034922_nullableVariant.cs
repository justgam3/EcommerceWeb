using Microsoft.EntityFrameworkCore.Migrations;

namespace EcommerceWebApi.Migrations
{
    public partial class nullableVariant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Variant_VariantID",
                table: "OrderDetail");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Variant_VariantID",
                table: "OrderDetail",
                column: "VariantID",
                principalTable: "Variant",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Variant_VariantID",
                table: "OrderDetail");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Variant_VariantID",
                table: "OrderDetail",
                column: "VariantID",
                principalTable: "Variant",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
