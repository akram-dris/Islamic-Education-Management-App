using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuranSchool.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntitiesToInheritFromEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubmittedAt",
                table: "Submissions",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Submissions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Submissions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "Submissions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "Submissions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Subjects",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Subjects",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Subjects",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "Subjects",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "Subjects",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ParentStudents",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "ParentStudents",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ParentStudents",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ParentStudents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "ParentStudents",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "ParentStudents",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Enrollments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Enrollments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Enrollments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "Enrollments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "Enrollments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Classes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Classes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Classes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "Classes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "Classes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AttendanceSessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "AttendanceSessions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AttendanceSessions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "AttendanceSessions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "AttendanceSessions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AttendanceRecords",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "AttendanceRecords",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AttendanceRecords",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "AttendanceRecords",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "AttendanceRecords",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Assignments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Assignments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "Assignments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "Assignments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Allocations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Allocations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Allocations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "Allocations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "Allocations",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ParentStudents");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ParentStudents");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ParentStudents");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ParentStudents");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "ParentStudents");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "ParentStudents");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AttendanceSessions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AttendanceSessions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AttendanceSessions");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "AttendanceSessions");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "AttendanceSessions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Allocations");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Allocations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Allocations");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "Allocations");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Allocations");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Submissions",
                newName: "SubmittedAt");
        }
    }
}
