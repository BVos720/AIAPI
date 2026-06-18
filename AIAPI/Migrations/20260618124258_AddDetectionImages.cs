using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDetectionImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DetectionImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DetectionId = table.Column<int>(type: "int", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetectionImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetectionImages_Detections_DetectionId",
                        column: x => x.DetectionId,
                        principalTable: "Detections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetectionImages_DetectionId",
                table: "DetectionImages",
                column: "DetectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetectionImages");
        }
    }
}
