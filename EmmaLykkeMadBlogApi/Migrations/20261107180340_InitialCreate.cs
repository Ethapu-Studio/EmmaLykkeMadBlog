using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EmmaLykkeMadBlogApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "category",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tag",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tag", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "recipe",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    short_description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    introduction = table.Column<string>(type: "text", nullable: true),
                    image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    prep_time_minutes = table.Column<int>(type: "integer", nullable: false),
                    cook_time_minutes = table.Column<int>(type: "integer", nullable: false),
                    servings = table.Column<int>(type: "integer", nullable: false),
                    calories = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    protein_g = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    carbs_g = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    fat_g = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    fiber_g = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    category_id = table.Column<int>(type: "integer", nullable: false),
                    is_published = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe", x => x.id);
                    table.ForeignKey(
                        name: "FK_recipe_category_category_id",
                        column: x => x.category_id,
                        principalTable: "category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recipe_ingredient",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    recipe_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    note = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_ingredient", x => x.id);
                    table.ForeignKey(
                        name: "FK_recipe_ingredient_recipe_recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipe",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recipe_step",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    recipe_id = table.Column<int>(type: "integer", nullable: false),
                    step_number = table.Column<int>(type: "integer", nullable: false),
                    instruction = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_step", x => x.id);
                    table.ForeignKey(
                        name: "FK_recipe_step_recipe_recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipe",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recipe_tag",
                columns: table => new
                {
                    recipe_id = table.Column<int>(type: "integer", nullable: false),
                    tag_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_tag", x => new { x.recipe_id, x.tag_id });
                    table.ForeignKey(
                        name: "FK_recipe_tag_recipe_recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipe",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_recipe_tag_tag_tag_id",
                        column: x => x.tag_id,
                        principalTable: "tag",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_category_name",
                table: "category",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_category_slug",
                table: "category",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_recipe_category_id",
                table: "recipe",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_slug",
                table: "recipe",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_recipe_ingredient_recipe_id",
                table: "recipe_ingredient",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_step_recipe_id_step_number",
                table: "recipe_step",
                columns: new[] { "recipe_id", "step_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_recipe_tag_tag_id",
                table: "recipe_tag",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "IX_tag_name",
                table: "tag",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tag_slug",
                table: "tag",
                column: "slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "recipe_ingredient");

            migrationBuilder.DropTable(
                name: "recipe_step");

            migrationBuilder.DropTable(
                name: "recipe_tag");

            migrationBuilder.DropTable(
                name: "recipe");

            migrationBuilder.DropTable(
                name: "tag");

            migrationBuilder.DropTable(
                name: "category");
        }
    }
}
