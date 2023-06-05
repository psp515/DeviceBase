using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceBaseApi.Migrations
{
    /// <inheritdoc />
    public partial class FixInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "DeviceTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Edited" },
                values: new object[] { new DateTime(2023, 6, 5, 12, 3, 29, 772, DateTimeKind.Local).AddTicks(6905), new DateTime(2023, 6, 5, 12, 3, 29, 772, DateTimeKind.Local).AddTicks(6944) });

            migrationBuilder.UpdateData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DeviceSecret", "Edited", "Produced" },
                values: new object[] { new DateTime(2023, 6, 5, 12, 3, 29, 772, DateTimeKind.Local).AddTicks(7119), "Secret", new DateTime(2023, 6, 5, 12, 3, 29, 772, DateTimeKind.Local).AddTicks(7121), new DateTime(2023, 6, 5, 12, 3, 29, 772, DateTimeKind.Local).AddTicks(7123) });

            migrationBuilder.UpdateData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "DeviceSecret", "Edited", "Produced" },
                values: new object[] { new DateTime(2023, 6, 5, 12, 3, 29, 772, DateTimeKind.Local).AddTicks(7128), "Secret", new DateTime(2023, 6, 5, 12, 3, 29, 772, DateTimeKind.Local).AddTicks(7129), new DateTime(2023, 6, 5, 12, 3, 29, 772, DateTimeKind.Local).AddTicks(7130) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "DeviceTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Edited" },
                values: new object[] { new DateTime(2023, 5, 10, 19, 53, 56, 664, DateTimeKind.Local).AddTicks(1105), new DateTime(2023, 5, 10, 19, 53, 56, 664, DateTimeKind.Local).AddTicks(1152) });

            migrationBuilder.UpdateData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DeviceSecret", "Edited", "Produced" },
                values: new object[] { new DateTime(2023, 5, 10, 19, 53, 56, 664, DateTimeKind.Local).AddTicks(1347), null, new DateTime(2023, 5, 10, 19, 53, 56, 664, DateTimeKind.Local).AddTicks(1350), new DateTime(2023, 5, 10, 19, 53, 56, 664, DateTimeKind.Local).AddTicks(1352) });

            migrationBuilder.UpdateData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "DeviceSecret", "Edited", "Produced" },
                values: new object[] { new DateTime(2023, 5, 10, 19, 53, 56, 664, DateTimeKind.Local).AddTicks(1359), null, new DateTime(2023, 5, 10, 19, 53, 56, 664, DateTimeKind.Local).AddTicks(1361), new DateTime(2023, 5, 10, 19, 53, 56, 664, DateTimeKind.Local).AddTicks(1362) });
        }
    }
}
