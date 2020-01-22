using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp1.Migrations
{
  public partial class SeedLanguage : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.Sql("INSERT INTO Languages (Id, Name) VALUES ('en', 'English')");
      migrationBuilder.Sql("INSERT INTO Languages (Id, Name) VALUES ('ar', 'Arabic')");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.Sql("DELETE FROM Languages WHERE Name IN ('English', 'Arabic')");
    }
  }
}
