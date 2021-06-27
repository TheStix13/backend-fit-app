using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace FitApp.Migrations
{
    public partial class initialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "exercises",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    exerciseName = table.Column<string>(maxLength: 32, nullable: false),
                    description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercises", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(maxLength: 100, nullable: false),
                    firstName = table.Column<string>(maxLength: 100, nullable: false),
                    secondName = table.Column<string>(maxLength: 100, nullable: false),
                    password = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "plans",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    planName = table.Column<string>(maxLength: 32, nullable: false),
                    description = table.Column<string>(nullable: false),
                    Userid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plans", x => x.id);
                    table.ForeignKey(
                        name: "FK_plans_users_Userid",
                        column: x => x.Userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "runScores",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    distance = table.Column<int>(nullable: false),
                    time = table.Column<int>(nullable: false),
                    Userid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_runScores", x => x.id);
                    table.ForeignKey(
                        name: "FK_runScores_users_Userid",
                        column: x => x.Userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "planExercises",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    series = table.Column<int>(nullable: false),
                    repetitions = table.Column<int>(nullable: false),
                    Exerciseid = table.Column<int>(nullable: true),
                    Planid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planExercises", x => x.id);
                    table.ForeignKey(
                        name: "FK_planExercises_exercises_Exerciseid",
                        column: x => x.Exerciseid,
                        principalTable: "exercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_planExercises_plans_Planid",
                        column: x => x.Planid,
                        principalTable: "plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "exerciseStatistics",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    series = table.Column<int>(nullable: false),
                    repetitions = table.Column<int>(nullable: false),
                    dateTime = table.Column<DateTime>(nullable: false),
                    PlanExerciseid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exerciseStatistics", x => x.id);
                    table.ForeignKey(
                        name: "FK_exerciseStatistics_planExercises_PlanExerciseid",
                        column: x => x.PlanExerciseid,
                        principalTable: "planExercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_exerciseStatistics_PlanExerciseid",
                table: "exerciseStatistics",
                column: "PlanExerciseid");

            migrationBuilder.CreateIndex(
                name: "IX_planExercises_Exerciseid",
                table: "planExercises",
                column: "Exerciseid");

            migrationBuilder.CreateIndex(
                name: "IX_planExercises_Planid",
                table: "planExercises",
                column: "Planid");

            migrationBuilder.CreateIndex(
                name: "IX_plans_Userid",
                table: "plans",
                column: "Userid");

            migrationBuilder.CreateIndex(
                name: "IX_runScores_Userid",
                table: "runScores",
                column: "Userid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exerciseStatistics");

            migrationBuilder.DropTable(
                name: "runScores");

            migrationBuilder.DropTable(
                name: "planExercises");

            migrationBuilder.DropTable(
                name: "exercises");

            migrationBuilder.DropTable(
                name: "plans");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
