using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceSimulator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initialize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "device_type",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device_type", x => x.Id);
                    table.UniqueConstraint("AK_device_type_Code", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "device",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Uri = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    DeviceTypeCode = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device", x => x.Id);
                    table.ForeignKey(
                        name: "FK_device_device_type_DeviceTypeCode",
                        column: x => x.DeviceTypeCode,
                        principalTable: "device_type",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "device_type",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { 1L, "10004", "Contactless VitalSign Monitoring Mattress" });

            migrationBuilder.CreateIndex(
                name: "IX_device_DeviceTypeCode",
                table: "device",
                column: "DeviceTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_device_Uri",
                table: "device",
                column: "Uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_device_type_Code",
                table: "device_type",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "device");

            migrationBuilder.DropTable(
                name: "device_type");
        }
    }
}
