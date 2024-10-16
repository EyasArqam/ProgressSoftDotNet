using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessCardAPI.Migrations
{
    public partial class AddIndexesOnNameDobPhoneGenderEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_BusinessCards_DateOfBirth",
                table: "BusinessCards",
                column: "DateOfBirth");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessCards_Email",
                table: "BusinessCards",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessCards_Gender",
                table: "BusinessCards",
                column: "Gender");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessCards_Name",
                table: "BusinessCards",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessCards_Phone",
                table: "BusinessCards",
                column: "Phone");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BusinessCards_DateOfBirth",
                table: "BusinessCards");

            migrationBuilder.DropIndex(
                name: "IX_BusinessCards_Email",
                table: "BusinessCards");

            migrationBuilder.DropIndex(
                name: "IX_BusinessCards_Gender",
                table: "BusinessCards");

            migrationBuilder.DropIndex(
                name: "IX_BusinessCards_Name",
                table: "BusinessCards");

            migrationBuilder.DropIndex(
                name: "IX_BusinessCards_Phone",
                table: "BusinessCards");
        }
    }
}
