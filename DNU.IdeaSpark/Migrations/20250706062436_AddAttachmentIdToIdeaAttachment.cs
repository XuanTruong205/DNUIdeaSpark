﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNU.IdeaSpark.Migrations
{
    /// <inheritdoc />
    public partial class AddAttachmentIdToIdeaAttachment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AwardedAt",
                table: "UserBadges",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UserBadgeId",
                table: "UserBadges",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AwardedAt",
                table: "UserBadges");

            migrationBuilder.DropColumn(
                name: "UserBadgeId",
                table: "UserBadges");
        }
    }
}
