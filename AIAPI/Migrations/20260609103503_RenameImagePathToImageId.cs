using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIAPI.Migrations
{
    /// <inheritdoc />
    public partial class RenameImagePathToImageId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "Detections",
                newName: "ImageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageId",
                table: "Detections",
                newName: "ImagePath");
        }
    }
}
