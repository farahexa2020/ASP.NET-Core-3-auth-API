using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp1.Data.Migrations
{
    public partial class AddInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Banks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageUrl = table.Column<string>(maxLength: 255, nullable: true),
                    AccountHolderName = table.Column<string>(maxLength: 255, nullable: false),
                    AccountNumber = table.Column<string>(maxLength: 255, nullable: false),
                    SequenceNumber = table.Column<int>(nullable: false),
                    SequenceNumberUpdatedAt = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageId = table.Column<string>(nullable: false),
                    LanguageId1 = table.Column<int>(nullable: true),
                    BankId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankTranslations_Banks_BankId",
                        column: x => x.BankId,
                        principalTable: "Banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BankTranslations_Languages_LanguageId1",
                        column: x => x.LanguageId1,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankTranslations_BankId",
                table: "BankTranslations",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_BankTranslations_LanguageId1",
                table: "BankTranslations",
                column: "LanguageId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankTranslations");

            migrationBuilder.DropTable(
                name: "Banks");

            migrationBuilder.DropTable(
                name: "Languages");
        }
    }
}
