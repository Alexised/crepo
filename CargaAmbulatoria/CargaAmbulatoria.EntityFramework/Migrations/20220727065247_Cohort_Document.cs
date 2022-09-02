using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargaAmbulatoria.EntityFramework.Migrations
{
    public partial class Cohort_Document : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "4be1c0b1-33bf-4401-8d77-b09b0064d0d6");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "8e4de531-ef35-4a04-a3a0-4b4530643c15");

            migrationBuilder.AddColumn<long>(
                name: "CohortId",
                table: "Documents",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Dni",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Regime",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Cohorts",
                columns: table => new
                {
                    CohortId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cohorts", x => x.CohortId);
                });

            migrationBuilder.InsertData(
                table: "Cohorts",
                columns: new[] { "CohortId", "Name" },
                values: new object[] { 1L, "ERC" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "Name", "PasswordStored", "Role", "Status", "TokenReset", "TokenResetExpiration" },
                values: new object[] { "302e8c06-302a-4e0b-9a19-d70a6541c655", "admin@coosalud.com", "Admin", "NhPGnizPnx4RQRSNWFXaRw==", 0, 0, null, null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "Name", "PasswordStored", "Role", "Status", "TokenReset", "TokenResetExpiration" },
                values: new object[] { "75ac2d91-4747-48d2-97c5-8d18e98fc58e", "agent@coosalud.com", "Agent", "NhPGnizPnx4RQRSNWFXaRw==", 1, 0, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CohortId",
                table: "Documents",
                column: "CohortId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Cohorts_CohortId",
                table: "Documents",
                column: "CohortId",
                principalTable: "Cohorts",
                principalColumn: "CohortId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Cohorts_CohortId",
                table: "Documents");

            migrationBuilder.DropTable(
                name: "Cohorts");

            migrationBuilder.DropIndex(
                name: "IX_Documents_CohortId",
                table: "Documents");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "302e8c06-302a-4e0b-9a19-d70a6541c655");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "75ac2d91-4747-48d2-97c5-8d18e98fc58e");

            migrationBuilder.DropColumn(
                name: "CohortId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Dni",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Regime",
                table: "Documents");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "Name", "PasswordStored", "Role", "Status", "TokenReset", "TokenResetExpiration" },
                values: new object[] { "4be1c0b1-33bf-4401-8d77-b09b0064d0d6", "admin@coosalud.com", "Admin", "NhPGnizPnx4RQRSNWFXaRw==", 0, 0, null, null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "Name", "PasswordStored", "Role", "Status", "TokenReset", "TokenResetExpiration" },
                values: new object[] { "8e4de531-ef35-4a04-a3a0-4b4530643c15", "agent@coosalud.com", "Agent", "NhPGnizPnx4RQRSNWFXaRw==", 1, 0, null, null });
        }
    }
}
