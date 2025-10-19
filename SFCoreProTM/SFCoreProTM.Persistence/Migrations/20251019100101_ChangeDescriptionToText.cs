using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SFCoreProTM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDescriptionToText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Mengubah tipe data kolom Description menjadi TEXT untuk tabel tasks
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "tasks",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(5000)",
                oldMaxLength: 5000);

            // Mengubah tipe data kolom Description menjadi TEXT untuk tabel projects
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "projects",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(5000)",
                oldMaxLength: 5000);

            // Mengubah tipe data kolom Description menjadi TEXT untuk tabel modules
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "modules",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(5000)",
                oldMaxLength: 5000);

            // Mengubah tipe data kolom Description menjadi TEXT untuk tabel flow_tasks
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "flow_tasks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            // Mengubah tipe data kolom Description menjadi TEXT untuk tabel erd_definitions
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "erd_definitions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Mengembalikan tipe data kolom Description ke VARCHAR untuk tabel tasks
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "tasks",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            // Mengembalikan tipe data kolom Description ke VARCHAR untuk tabel projects
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "projects",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            // Mengembalikan tipe data kolom Description ke VARCHAR untuk tabel modules
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "modules",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            // Mengembalikan tipe data kolom Description ke VARCHAR untuk tabel flow_tasks
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "flow_tasks",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            // Mengembalikan tipe data kolom Description ke VARCHAR untuk tabel erd_definitions
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "erd_definitions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}