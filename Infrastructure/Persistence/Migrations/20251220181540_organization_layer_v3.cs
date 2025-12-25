using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class organization_layer_v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomProfiles_Profiles_ProfileId",
                table: "RoomProfiles");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomProfiles_Profiles_ProfileId",
                table: "RoomProfiles",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomProfiles_Profiles_ProfileId",
                table: "RoomProfiles");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomProfiles_Profiles_ProfileId",
                table: "RoomProfiles",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
