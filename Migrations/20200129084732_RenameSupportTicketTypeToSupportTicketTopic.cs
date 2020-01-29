using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp1.Migrations
{
    public partial class RenameSupportTicketTypeToSupportTicketTopic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportTickets_SupportTicketTypes_TypeId",
                table: "SupportTickets");

            migrationBuilder.DropTable(
                name: "SupportTicketTypes");

            migrationBuilder.CreateTable(
                name: "SupportTicketTopics",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Description = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTicketTopics", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_SupportTickets_SupportTicketTopics_TypeId",
                table: "SupportTickets",
                column: "TypeId",
                principalTable: "SupportTicketTopics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportTickets_SupportTicketTopics_TypeId",
                table: "SupportTickets");

            migrationBuilder.DropTable(
                name: "SupportTicketTopics");

            migrationBuilder.CreateTable(
                name: "SupportTicketTypes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTicketTypes", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_SupportTickets_SupportTicketTypes_TypeId",
                table: "SupportTickets",
                column: "TypeId",
                principalTable: "SupportTicketTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
