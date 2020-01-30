using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp1.Migrations
{
    public partial class UpdateSupportTicketModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportTickets_SupportTicketPriorities_SupportTicketPriorityId",
                table: "SupportTickets");

            migrationBuilder.DropIndex(
                name: "IX_SupportTickets_SupportTicketPriorityId",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "SupportTicketPriorityId",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SupportTickets");

            migrationBuilder.AlterColumn<string>(
                name: "AssigneeId",
                table: "SupportTickets",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "PriorityId",
                table: "SupportTickets",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_PriorityId",
                table: "SupportTickets",
                column: "PriorityId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupportTickets_SupportTicketPriorities_PriorityId",
                table: "SupportTickets",
                column: "PriorityId",
                principalTable: "SupportTicketPriorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportTickets_SupportTicketPriorities_PriorityId",
                table: "SupportTickets");

            migrationBuilder.DropIndex(
                name: "IX_SupportTickets_PriorityId",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "PriorityId",
                table: "SupportTickets");

            migrationBuilder.AlterColumn<string>(
                name: "AssigneeId",
                table: "SupportTickets",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupportTicketPriorityId",
                table: "SupportTickets",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "SupportTickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_SupportTicketPriorityId",
                table: "SupportTickets",
                column: "SupportTicketPriorityId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupportTickets_SupportTicketPriorities_SupportTicketPriorityId",
                table: "SupportTickets",
                column: "SupportTicketPriorityId",
                principalTable: "SupportTicketPriorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
