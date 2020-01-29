using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp1.Migrations
{
    public partial class AddTicketResponseAndMessageAddFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportTickets_SupportTicketTopics_TypeId",
                table: "SupportTickets");

            migrationBuilder.DropIndex(
                name: "IX_SupportTickets_TypeId",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "SupportTickets");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "SupportTicketTopics",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "SupportTicketStatuses",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssigneeId",
                table: "SupportTickets",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "SupportTickets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupportTicketPriorityId",
                table: "SupportTickets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TopicId",
                table: "SupportTickets",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "SupportTickets",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SupportTicketMessages",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    TicketId = table.Column<string>(nullable: false),
                    PostedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTicketMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportTicketMessages_SupportTickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "SupportTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupportTicketMessages_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupportTicketPriorities",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTicketPriorities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupportTicketResponses",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    TicketId = table.Column<string>(nullable: false),
                    PostedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTicketResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportTicketResponses_SupportTickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "SupportTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupportTicketResponses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_AssigneeId",
                table: "SupportTickets",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_SupportTicketPriorityId",
                table: "SupportTickets",
                column: "SupportTicketPriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_TopicId",
                table: "SupportTickets",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTicketMessages_TicketId",
                table: "SupportTicketMessages",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTicketMessages_UserId",
                table: "SupportTicketMessages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTicketResponses_TicketId",
                table: "SupportTicketResponses",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTicketResponses_UserId",
                table: "SupportTicketResponses",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupportTickets_AspNetUsers_AssigneeId",
                table: "SupportTickets",
                column: "AssigneeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupportTickets_SupportTicketPriorities_SupportTicketPriorityId",
                table: "SupportTickets",
                column: "SupportTicketPriorityId",
                principalTable: "SupportTicketPriorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupportTickets_SupportTicketTopics_TopicId",
                table: "SupportTickets",
                column: "TopicId",
                principalTable: "SupportTicketTopics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportTickets_AspNetUsers_AssigneeId",
                table: "SupportTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_SupportTickets_SupportTicketPriorities_SupportTicketPriorityId",
                table: "SupportTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_SupportTickets_SupportTicketTopics_TopicId",
                table: "SupportTickets");

            migrationBuilder.DropTable(
                name: "SupportTicketMessages");

            migrationBuilder.DropTable(
                name: "SupportTicketPriorities");

            migrationBuilder.DropTable(
                name: "SupportTicketResponses");

            migrationBuilder.DropIndex(
                name: "IX_SupportTickets_AssigneeId",
                table: "SupportTickets");

            migrationBuilder.DropIndex(
                name: "IX_SupportTickets_SupportTicketPriorityId",
                table: "SupportTickets");

            migrationBuilder.DropIndex(
                name: "IX_SupportTickets_TopicId",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SupportTicketTopics");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SupportTicketStatuses");

            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "SupportTicketPriorityId",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "TopicId",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SupportTickets");

            migrationBuilder.AddColumn<string>(
                name: "TypeId",
                table: "SupportTickets",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_TypeId",
                table: "SupportTickets",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupportTickets_SupportTicketTopics_TypeId",
                table: "SupportTickets",
                column: "TypeId",
                principalTable: "SupportTicketTopics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
