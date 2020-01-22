using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp1.Migrations
{
    public partial class UpdateBankTranslationKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BankTranslations",
                table: "BankTranslations");

            migrationBuilder.DropIndex(
                name: "IX_BankTranslations_LanguageId",
                table: "BankTranslations");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "BankTranslations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BankTranslations",
                table: "BankTranslations",
                columns: new[] { "LanguageId", "BankId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BankTranslations",
                table: "BankTranslations");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "BankTranslations",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BankTranslations",
                table: "BankTranslations",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BankTranslations_LanguageId",
                table: "BankTranslations",
                column: "LanguageId");
        }
    }
}
