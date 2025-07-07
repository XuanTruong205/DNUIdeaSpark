﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNU.IdeaSpark.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalPointsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalPoints",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPoints",
                table: "Users");
        }
    }
}
