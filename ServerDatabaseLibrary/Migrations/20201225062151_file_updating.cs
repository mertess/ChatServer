using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ServerDatabaseSystem.Migrations
{
    public partial class file_updating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "Users");

            migrationBuilder.AddColumn<byte[]>(
                name: "Picture",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PictureExtension",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PictureName",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "File",
                table: "Messages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileExtension",
                table: "Messages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Messages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Picture",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PictureExtension",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PictureName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "File",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "FileExtension",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Messages");

            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BinaryForm = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Extension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileId = table.Column<int>(type: "int", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_Users_FileId",
                        column: x => x.FileId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Files_FileId",
                table: "Files",
                column: "FileId",
                unique: true,
                filter: "[FileId] IS NOT NULL");
        }
    }
}
