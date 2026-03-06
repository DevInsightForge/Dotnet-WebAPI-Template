using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevInsightForge.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IsEmailVerified = table.Column<bool>(type: "boolean", nullable: false),
                    DateJoined = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DateJoined", "Email", "IsEmailVerified", "LastLogin", "NormalizedEmail", "PasswordHash" },
                values: new object[] { new Guid("019cc42b-1d4a-7e16-886c-5267c7e96651"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@default.local", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ADMIN@DEFAULT.LOCAL", "$argon2id$v=19$m=65536,t=3,p=1$Xka0Ez/kddlgKLbErxj7Ng$mBT9xHzRHIhVfsL3kV79DzB2TIL/mMhXp5SbVHBMzTc" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_NormalizedEmail",
                table: "Users",
                column: "NormalizedEmail",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
