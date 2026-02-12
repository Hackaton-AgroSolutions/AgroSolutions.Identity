using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroSolutions.Identity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedingDemoUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "Name", "Password" },
                values: new object[] { 1, "demo@gmail.com", "Demo User", "$2a$12$2Dj1BaOnV8X0ej7U0KIOjOneac1OOcv9L8rhoIbOgSiafuPPnwQIi" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);
        }
    }
}
