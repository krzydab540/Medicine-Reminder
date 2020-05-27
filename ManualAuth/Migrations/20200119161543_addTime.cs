using Microsoft.EntityFrameworkCore.Migrations;

namespace ManualAuth.Migrations
{
    public partial class addTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Id_patient",
                table: "Pressure",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "HourOfTaking",
                table: "Medicine",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinuteOfTaking",
                table: "Medicine",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Id_patient",
                table: "Glucoses",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HourOfTaking",
                table: "Medicine");

            migrationBuilder.DropColumn(
                name: "MinuteOfTaking",
                table: "Medicine");

            migrationBuilder.AlterColumn<string>(
                name: "Id_patient",
                table: "Pressure",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id_patient",
                table: "Glucoses",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
