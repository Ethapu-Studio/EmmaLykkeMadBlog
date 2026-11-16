using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmmaLykkeMadBlogApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountFieldsAndAdminRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "app_user",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "display_name",
                table: "app_user",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "newsletter_opt_in_at",
                table: "app_user",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                table: "app_user");

            migrationBuilder.DropColumn(
                name: "display_name",
                table: "app_user");

            migrationBuilder.DropColumn(
                name: "newsletter_opt_in_at",
                table: "app_user");
        }
    }
}
