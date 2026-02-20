using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Brotherhood_Portal.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RebuildSchema2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinanceApproval",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FinanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceApproval", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinanceApproval_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FinanceApproval_Finances_FinanceId",
                        column: x => x.FinanceId,
                        principalTable: "Finances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinanceApproval_FinanceId",
                table: "FinanceApproval",
                column: "FinanceId");

            migrationBuilder.CreateIndex(
                name: "IX_FinanceApproval_UserId",
                table: "FinanceApproval",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinanceApproval");
        }
    }
}
