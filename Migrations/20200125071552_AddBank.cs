using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp1.Migrations
{
    public partial class AddBank : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankTranslations_Banks_BankId",
                table: "BankTranslations");

            migrationBuilder.DropForeignKey(
                name: "FK_BankTranslations_Languages_LanguageId",
                table: "BankTranslations");

            migrationBuilder.AddForeignKey(
                name: "FK_BankTranslations_Banks_BankId",
                table: "BankTranslations",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BankTranslations_Languages_LanguageId",
                table: "BankTranslations",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankTranslations_Banks_BankId",
                table: "BankTranslations");

            migrationBuilder.DropForeignKey(
                name: "FK_BankTranslations_Languages_LanguageId",
                table: "BankTranslations");

            migrationBuilder.AddForeignKey(
                name: "FK_BankTranslations_Banks_BankId",
                table: "BankTranslations",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankTranslations_Languages_LanguageId",
                table: "BankTranslations",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
