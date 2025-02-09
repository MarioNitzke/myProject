using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITnetworkProjekt.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInsuredPersonIdDescriptionOfInsurance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Insurance_InsuredPerson_InsuredPersonID",
                table: "Insurance");

            migrationBuilder.RenameColumn(
                name: "InsuredPersonID",
                table: "Insurance",
                newName: "InsuredPersonId");

            migrationBuilder.RenameIndex(
                name: "IX_Insurance_InsuredPersonID",
                table: "Insurance",
                newName: "IX_Insurance_InsuredPersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Insurance_InsuredPerson_InsuredPersonId",
                table: "Insurance",
                column: "InsuredPersonId",
                principalTable: "InsuredPerson",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Insurance_InsuredPerson_InsuredPersonId",
                table: "Insurance");

            migrationBuilder.RenameColumn(
                name: "InsuredPersonId",
                table: "Insurance",
                newName: "InsuredPersonID");

            migrationBuilder.RenameIndex(
                name: "IX_Insurance_InsuredPersonId",
                table: "Insurance",
                newName: "IX_Insurance_InsuredPersonID");

            migrationBuilder.AddForeignKey(
                name: "FK_Insurance_InsuredPerson_InsuredPersonID",
                table: "Insurance",
                column: "InsuredPersonID",
                principalTable: "InsuredPerson",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
