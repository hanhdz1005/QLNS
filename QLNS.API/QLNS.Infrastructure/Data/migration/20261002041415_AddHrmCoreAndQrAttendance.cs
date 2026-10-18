using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLNS.Infrastructure.Data.migration
{
    /// <inheritdoc />
    public partial class AddHrmCoreAndQrAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LeaveType",
                table: "LEAVE_REQUESTS",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BankAccountNumber",
                table: "EMPLOYEES",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankBranch",
                table: "EMPLOYEES",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "EMPLOYEES",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CitizenId",
                table: "EMPLOYEES",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "CitizenIdIssueDate",
                table: "EMPLOYEES",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CitizenIdIssuePlace",
                table: "EMPLOYEES",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DependentsCount",
                table: "EMPLOYEES",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactName",
                table: "EMPLOYEES",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactPhone",
                table: "EMPLOYEES",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactRelationship",
                table: "EMPLOYEES",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ManagerId",
                table: "EMPLOYEES",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SocialInsuranceNumber",
                table: "EMPLOYEES",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxCode",
                table: "EMPLOYEES",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CONTRACTS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContractNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContractType = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    SignDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ContractSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InsuranceSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AttachmentUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONTRACTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CONTRACTS_EMPLOYEES_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EMPLOYEES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LEAVE_BALANCES",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    TotalLeaveDays = table.Column<decimal>(type: "decimal(4,1)", nullable: false),
                    UsedLeaveDays = table.Column<decimal>(type: "decimal(4,1)", nullable: false),
                    RemainingLeaveDays = table.Column<decimal>(type: "decimal(4,1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LEAVE_BALANCES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LEAVE_BALANCES_EMPLOYEES_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EMPLOYEES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEES_ManagerId",
                table: "EMPLOYEES",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_CONTRACTS_EmployeeId",
                table: "CONTRACTS",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LEAVE_BALANCES_EmployeeId_Year",
                table: "LEAVE_BALANCES",
                columns: new[] { "EmployeeId", "Year" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EMPLOYEES_EMPLOYEES_ManagerId",
                table: "EMPLOYEES",
                column: "ManagerId",
                principalTable: "EMPLOYEES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EMPLOYEES_EMPLOYEES_ManagerId",
                table: "EMPLOYEES");

            migrationBuilder.DropTable(
                name: "CONTRACTS");

            migrationBuilder.DropTable(
                name: "LEAVE_BALANCES");

            migrationBuilder.DropIndex(
                name: "IX_EMPLOYEES_ManagerId",
                table: "EMPLOYEES");

            migrationBuilder.DropColumn(
                name: "LeaveType",
                table: "LEAVE_REQUESTS");

            migrationBuilder.DropColumn(
                name: "BankAccountNumber",
                table: "EMPLOYEES");

            migrationBuilder.DropColumn(
                name: "BankBranch",
                table: "EMPLOYEES");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "EMPLOYEES");

            migrationBuilder.DropColumn(
                name: "CitizenId",
                table: "EMPLOYEES");

            migrationBuilder.DropColumn(
                name: "CitizenIdIssueDate",
                table: "EMPLOYEES");

            migrationBuilder.DropColumn(
                name: "CitizenIdIssuePlace",
                table: "EMPLOYEES");

            migrationBuilder.DropColumn(
                name: "DependentsCount",
                table: "EMPLOYEES");

            migrationBuilder.DropColumn(
                name: "EmergencyContactName",
                table: "EMPLOYEES");

            migrationBuilder.DropColumn(
                name: "EmergencyContactPhone",
                table: "EMPLOYEES");

            migrationBuilder.DropColumn(
                name: "EmergencyContactRelationship",
                table: "EMPLOYEES");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "EMPLOYEES");

            migrationBuilder.DropColumn(
                name: "SocialInsuranceNumber",
                table: "EMPLOYEES");

            migrationBuilder.DropColumn(
                name: "TaxCode",
                table: "EMPLOYEES");
        }
    }
}
