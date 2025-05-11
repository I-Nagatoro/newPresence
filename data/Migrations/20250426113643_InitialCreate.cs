using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public2");

            migrationBuilder.CreateTable(
                name: "groups",
                schema: "public2",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groups", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "public2",
                columns: table => new
                {
                    userid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fio = table.Column<string>(type: "text", nullable: false),
                    groupid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.userid);
                    table.ForeignKey(
                        name: "FK_users_groups_groupid",
                        column: x => x.groupid,
                        principalSchema: "public2",
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "presence",
                schema: "public2",
                columns: table => new
                {
                    presenceid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    lessonnumber = table.Column<int>(type: "integer", nullable: false),
                    isattendance = table.Column<bool>(type: "boolean", nullable: false),
                    groupid = table.Column<int>(type: "integer", nullable: false),
                    UserDAOUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_presence", x => x.presenceid);
                    table.ForeignKey(
                        name: "FK_presence_users_UserDAOUserId",
                        column: x => x.UserDAOUserId,
                        principalSchema: "public2",
                        principalTable: "users",
                        principalColumn: "userid");
                });

            migrationBuilder.CreateIndex(
                name: "IX_presence_UserDAOUserId",
                schema: "public2",
                table: "presence",
                column: "UserDAOUserId");

            migrationBuilder.CreateIndex(
                name: "IX_users_groupid",
                schema: "public2",
                table: "users",
                column: "groupid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "presence",
                schema: "public2");

            migrationBuilder.DropTable(
                name: "users",
                schema: "public2");

            migrationBuilder.DropTable(
                name: "groups",
                schema: "public2");
        }
    }
}
