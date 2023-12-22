using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceSimulator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Device_Type_Seeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "device_type",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Name",
                value: "mattress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "device_type",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Name",
                value: "Contactless VitalSign Monitoring Mattress");
        }
    }
}
