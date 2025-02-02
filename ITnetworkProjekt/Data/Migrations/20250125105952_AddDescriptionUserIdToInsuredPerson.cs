using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITnetworkProjekt.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionUserIdToInsuredPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "InsuredPerson",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "InsuredPerson");
        }
    }
}


