using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedKeyToDoctorEducation2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DoctorEducation",
                table: "DoctorEducation");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "DoctorEducation",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DoctorEducation",
                table: "DoctorEducation",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorEducation_DoctorId",
                table: "DoctorEducation",
                column: "DoctorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DoctorEducation",
                table: "DoctorEducation");

            migrationBuilder.DropIndex(
                name: "IX_DoctorEducation_DoctorId",
                table: "DoctorEducation");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "DoctorEducation",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DoctorEducation",
                table: "DoctorEducation",
                columns: new[] { "DoctorId", "Id" });
        }
    }
}
