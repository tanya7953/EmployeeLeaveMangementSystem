using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeLeaveMangementSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Leaves",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Leaves");
        }
    }
}
