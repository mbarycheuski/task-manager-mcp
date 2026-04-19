using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Api.Data.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ApiKeys",
            columns: table => new
            {
                Id = table.Column<Guid>(
                    type: "uuid",
                    nullable: false,
                    defaultValueSql: "gen_random_uuid()"
                ),
                ClientName = table.Column<string>(
                    type: "character varying(100)",
                    maxLength: 100,
                    nullable: false
                ),
                KeyHash = table.Column<string>(
                    type: "character varying(128)",
                    maxLength: 128,
                    nullable: false
                ),
                Salt = table.Column<string>(
                    type: "character varying(64)",
                    maxLength: 64,
                    nullable: false
                ),
                CreatedAt = table.Column<DateTime>(
                    type: "timestamp with time zone",
                    nullable: false,
                    defaultValueSql: "now() at time zone 'utc'"
                ),
                IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ApiKeys", x => x.Id);
            }
        );

        migrationBuilder.CreateTable(
            name: "Tasks",
            columns: table => new
            {
                Id = table.Column<Guid>(
                    type: "uuid",
                    nullable: false,
                    defaultValueSql: "gen_random_uuid()"
                ),
                Title = table.Column<string>(
                    type: "character varying(255)",
                    maxLength: 255,
                    nullable: false
                ),
                Notes = table.Column<string>(
                    type: "character varying(4000)",
                    maxLength: 4000,
                    nullable: true
                ),
                Priority = table.Column<int>(type: "integer", nullable: true),
                Status = table.Column<int>(type: "integer", nullable: false),
                DueDate = table.Column<DateOnly>(type: "date", nullable: true),
                CreatedAt = table.Column<DateTime>(
                    type: "timestamp with time zone",
                    nullable: false,
                    defaultValueSql: "now() at time zone 'utc'"
                ),
                UpdatedAt = table.Column<DateTime>(
                    type: "timestamp with time zone",
                    nullable: false,
                    defaultValueSql: "now() at time zone 'utc'"
                ),
                CompletedAt = table.Column<DateTime>(
                    type: "timestamp with time zone",
                    nullable: true
                ),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Tasks", x => x.Id);
            }
        );
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ApiKeys");

        migrationBuilder.DropTable(name: "Tasks");
    }
}
