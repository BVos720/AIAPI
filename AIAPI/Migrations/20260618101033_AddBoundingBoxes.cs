using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddBoundingBoxes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BoundingBoxes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrainingImageId = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CenterX = table.Column<float>(type: "real", nullable: false),
                    CenterY = table.Column<float>(type: "real", nullable: false),
                    Width = table.Column<float>(type: "real", nullable: false),
                    Height = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoundingBoxes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoundingBoxes_TrainingImages_TrainingImageId",
                        column: x => x.TrainingImageId,
                        principalTable: "TrainingImages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoundingBoxes_TrainingImageId",
                table: "BoundingBoxes",
                column: "TrainingImageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoundingBoxes");
        }
    }
}
