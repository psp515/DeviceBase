using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeviceBaseApi.Migrations
{
    /// <inheritdoc />
    public partial class TypeInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeviceUser_Devices_DevicesDeviceId",
                table: "DeviceUser");

            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Devices",
                table: "Devices");

            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "DeviceId",
                keyValue: 1);

            migrationBuilder.RenameColumn(
                name: "DevicesDeviceId",
                table: "DeviceUser",
                newName: "DevicesId");

            migrationBuilder.RenameColumn(
                name: "DeviceId",
                table: "Devices",
                newName: "DeviceTypeId");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceTypeId",
                table: "Devices",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Devices",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Devices",
                table: "Devices",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "DeviceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DefaultName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaximalNumberOfUsers = table.Column<int>(type: "int", nullable: false),
                    EndpointsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Edited = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "DeviceTypes",
                columns: new[] { "Id", "Created", "DefaultName", "Edited", "EndpointsJson", "MaximalNumberOfUsers" },
                values: new object[] { 1, new DateTime(2023, 4, 28, 18, 6, 50, 229, DateTimeKind.Local).AddTicks(2360), "SP611", new DateTime(2023, 4, 28, 18, 6, 50, 229, DateTimeKind.Local).AddTicks(2450), "[\"state\",\"mode\",\"ping\"]", 5 });

            migrationBuilder.InsertData(
                table: "Devices",
                columns: new[] { "Id", "Created", "Description", "DeviceName", "DevicePlacing", "DeviceTypeId", "Edited", "MqttUrl", "Produced", "SerialNumber" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 4, 28, 18, 6, 50, 229, DateTimeKind.Local).AddTicks(2660), "", "SP611", "None", 1, new DateTime(2023, 4, 28, 18, 6, 50, 229, DateTimeKind.Local).AddTicks(2660), "https://www.google.pl/", new DateTime(2023, 4, 28, 18, 6, 50, 229, DateTimeKind.Local).AddTicks(2660), "21371" },
                    { 2, new DateTime(2023, 4, 28, 18, 6, 50, 229, DateTimeKind.Local).AddTicks(2670), "", "SP611", "None", 1, new DateTime(2023, 4, 28, 18, 6, 50, 229, DateTimeKind.Local).AddTicks(2670), "https://www.hivemq.com/", new DateTime(2023, 4, 28, 18, 6, 50, 229, DateTimeKind.Local).AddTicks(2680), "21372" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_DeviceTypeId",
                table: "Devices",
                column: "DeviceTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_DeviceTypes_DeviceTypeId",
                table: "Devices",
                column: "DeviceTypeId",
                principalTable: "DeviceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceUser_Devices_DevicesId",
                table: "DeviceUser",
                column: "DevicesId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_DeviceTypes_DeviceTypeId",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_DeviceUser_Devices_DevicesId",
                table: "DeviceUser");

            migrationBuilder.DropTable(
                name: "DeviceTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Devices",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_DeviceTypeId",
                table: "Devices");

            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Devices");

            migrationBuilder.RenameColumn(
                name: "DevicesId",
                table: "DeviceUser",
                newName: "DevicesDeviceId");

            migrationBuilder.RenameColumn(
                name: "DeviceTypeId",
                table: "Devices",
                newName: "DeviceId");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "Devices",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Devices",
                table: "Devices",
                column: "DeviceId");

            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Percent = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "Id", "Created", "IsActive", "LastUpdated", "Name", "Percent" },
                values: new object[,]
                {
                    { 1, null, true, null, "10OFF", 10 },
                    { 2, null, true, null, "20OFF", 20 }
                });

            migrationBuilder.InsertData(
                table: "Devices",
                columns: new[] { "DeviceId", "Created", "Description", "DeviceName", "DevicePlacing", "Edited", "MqttUrl", "Produced", "SerialNumber" },
                values: new object[] { 1, new DateTime(2023, 4, 17, 11, 44, 14, 790, DateTimeKind.Local).AddTicks(5191), null, null, null, new DateTime(2023, 4, 17, 11, 44, 14, 790, DateTimeKind.Local).AddTicks(5234), "https://www.google.pl/", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "1094205034" });

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceUser_Devices_DevicesDeviceId",
                table: "DeviceUser",
                column: "DevicesDeviceId",
                principalTable: "Devices",
                principalColumn: "DeviceId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
