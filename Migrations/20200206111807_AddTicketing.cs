using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp1.Migrations
{
    public partial class AddTicketing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    SenderId = table.Column<string>(nullable: true),
                    RecieverId = table.Column<string>(nullable: true),
                    message = table.Column<string>(nullable: true),
                    seen = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupportTicketPriorities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTicketPriorities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupportTicketStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTicketStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupportTicketTopics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTicketTopics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupportTickets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Issue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    AssigneeId = table.Column<string>(nullable: true),
                    TopicId = table.Column<int>(nullable: false),
                    StatusId = table.Column<int>(nullable: false),
                    PriorityId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportTickets_AspNetUsers_AssigneeId",
                        column: x => x.AssigneeId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupportTickets_SupportTicketPriorities_PriorityId",
                        column: x => x.PriorityId,
                        principalTable: "SupportTicketPriorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupportTickets_SupportTicketStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "SupportTicketStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupportTickets_SupportTicketTopics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "SupportTicketTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupportTickets_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupportTicketResponses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    TicketId = table.Column<int>(nullable: false),
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
                name: "IX_SupportTicketResponses_TicketId",
                table: "SupportTicketResponses",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTicketResponses_UserId",
                table: "SupportTicketResponses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_AssigneeId",
                table: "SupportTickets",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_PriorityId",
                table: "SupportTickets",
                column: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_StatusId",
                table: "SupportTickets",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_TopicId",
                table: "SupportTickets",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_UserId",
                table: "SupportTickets",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "SupportTicketResponses");

            migrationBuilder.DropTable(
                name: "SupportTickets");

            migrationBuilder.DropTable(
                name: "SupportTicketPriorities");

            migrationBuilder.DropTable(
                name: "SupportTicketStatuses");

            migrationBuilder.DropTable(
                name: "SupportTicketTopics");
        }
    }
}
