using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargaAmbulatoria.EntityFramework.Migrations
{
    public partial class AddPasswordTries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "302e8c06-302a-4e0b-9a19-d70a6541c655");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "75ac2d91-4747-48d2-97c5-8d18e98fc58e");

            migrationBuilder.AddColumn<int>(
                name: "PasswordTries",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "Name", "PasswordStored", "PasswordTries", "Role", "Status", "TokenReset", "TokenResetExpiration" },
                values: new object[] { "4838d0f2-319f-4fe6-b442-5528d91827c3", "admin@coosalud.com", "Admin", "NhPGnizPnx4RQRSNWFXaRw==", 0, 0, 0, null, null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "Name", "PasswordStored", "PasswordTries", "Role", "Status", "TokenReset", "TokenResetExpiration" },
                values: new object[] { "c54b49ae-29b8-48e8-b31b-170977c51ef4", "agent@coosalud.com", "Agent", "NhPGnizPnx4RQRSNWFXaRw==", 0, 1, 0, null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "4838d0f2-319f-4fe6-b442-5528d91827c3");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "c54b49ae-29b8-48e8-b31b-170977c51ef4");

            migrationBuilder.DropColumn(
                name: "PasswordTries",
                table: "Users");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "Name", "PasswordStored", "Role", "Status", "TokenReset", "TokenResetExpiration" },
                values: new object[] { "302e8c06-302a-4e0b-9a19-d70a6541c655", "admin@coosalud.com", "Admin", "NhPGnizPnx4RQRSNWFXaRw==", 0, 0, null, null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "Name", "PasswordStored", "Role", "Status", "TokenReset", "TokenResetExpiration" },
                values: new object[] { "75ac2d91-4747-48d2-97c5-8d18e98fc58e", "agent@coosalud.com", "Agent", "NhPGnizPnx4RQRSNWFXaRw==", 1, 0, null, null });
        }
    }
}
