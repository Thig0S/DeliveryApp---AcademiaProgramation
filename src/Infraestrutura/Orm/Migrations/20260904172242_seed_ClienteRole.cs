using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryApp.Infraestrutura.Orm.Migrations
{
    /// <inheritdoc />
    public partial class seed_ClienteRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("01a0651a-a522-7a83-a062-033b797331d0"), "01a0651d-7402-7053-874c-fe91e0612b5a", "Cliente", "CLIENTE" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("01a0651a-a522-7a83-a062-033b797331d0"));
        }
    }
}
