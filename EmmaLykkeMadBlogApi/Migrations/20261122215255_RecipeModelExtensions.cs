using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmmaLykkeMadBlogApi.Migrations
{
    /// <inheritdoc />
    public partial class RecipeModelExtensions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "image_url",
                table: "recipe",
                newName: "image_path");

            migrationBuilder.RenameColumn(
                name: "fiber_g",
                table: "recipe",
                newName: "sugar_g");

            migrationBuilder.AddColumn<string>(
                name: "group_name",
                table: "recipe_ingredient",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "favorite_sort_order",
                table: "recipe",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_alt",
                table: "recipe",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_favorite",
                table: "recipe",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "published_at",
                table: "recipe",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "salt_g",
                table: "recipe",
                type: "numeric(8,2)",
                precision: 8,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "saturated_fat_g",
                table: "recipe",
                type: "numeric(8,2)",
                precision: 8,
                scale: 2,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "popular_recipe",
                columns: table => new
                {
                    recipe_id = table.Column<int>(type: "integer", nullable: false),
                    rank = table.Column<int>(type: "integer", nullable: false),
                    score = table.Column<int>(type: "integer", nullable: false),
                    calculated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_popular_recipe", x => x.recipe_id);
                    table.ForeignKey(
                        name: "FK_popular_recipe_recipe_recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipe",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recipe_view_daily",
                columns: table => new
                {
                    recipe_id = table.Column<int>(type: "integer", nullable: false),
                    view_date = table.Column<DateOnly>(type: "date", nullable: false),
                    views = table.Column<int>(type: "integer", nullable: false),
                    unique_visitors = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_view_daily", x => new { x.recipe_id, x.view_date });
                    table.ForeignKey(
                        name: "FK_recipe_view_daily_recipe_recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipe",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_recipe_is_published_published_at",
                table: "recipe",
                columns: new[] { "is_published", "published_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "popular_recipe");

            migrationBuilder.DropTable(
                name: "recipe_view_daily");

            migrationBuilder.DropIndex(
                name: "IX_recipe_is_published_published_at",
                table: "recipe");

            migrationBuilder.DropColumn(
                name: "group_name",
                table: "recipe_ingredient");

            migrationBuilder.DropColumn(
                name: "favorite_sort_order",
                table: "recipe");

            migrationBuilder.DropColumn(
                name: "image_alt",
                table: "recipe");

            migrationBuilder.DropColumn(
                name: "is_favorite",
                table: "recipe");

            migrationBuilder.DropColumn(
                name: "published_at",
                table: "recipe");

            migrationBuilder.DropColumn(
                name: "salt_g",
                table: "recipe");

            migrationBuilder.DropColumn(
                name: "saturated_fat_g",
                table: "recipe");

            migrationBuilder.RenameColumn(
                name: "sugar_g",
                table: "recipe",
                newName: "fiber_g");

            migrationBuilder.RenameColumn(
                name: "image_path",
                table: "recipe",
                newName: "image_url");
        }
    }
}
