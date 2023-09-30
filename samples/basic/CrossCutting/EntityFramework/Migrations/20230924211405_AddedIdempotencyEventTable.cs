using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrossCutting.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddedIdempotencyEventTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IdempotencyEvents",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "TEXT", nullable: false),
                    HandlerId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdempotencyEvents", x => new { x.EventId, x.HandlerId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdempotencyEvents");
        }
    }
}
