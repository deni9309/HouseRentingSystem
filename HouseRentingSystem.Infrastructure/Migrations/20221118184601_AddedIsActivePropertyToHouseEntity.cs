using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HouseRentingSystem.Infrastructure.Migrations
{
    public partial class AddedIsActivePropertyToHouseEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Houses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            //migrationBuilder.UpdateData(
            //    table: "AspNetUsers",
            //    keyColumn: "Id",
            //    keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
            //    columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
            //    values: new object[] { "493af913-9121-4c3e-903d-630f37a6ccf5", "AQAAAAEAACcQAAAAEGyC7g3e9g9bcHWOCNMXIcNHgepTphX+hdjQYj4OaG+l38x4AVnJX4mKPspWLdRPZw==", "759c8783-b82a-4ea6-a29e-a38792b83f0c" });

            //migrationBuilder.UpdateData(
            //    table: "AspNetUsers",
            //    keyColumn: "Id",
            //    keyValue: "dea12856-c198-4129-b3f3-b893d8395082",
            //    columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
            //    values: new object[] { "b4c3692a-c145-4aef-9451-fd9864614f8e", "AQAAAAEAACcQAAAAENFezziAanUadyCx8+2jQlh4SBb7+tLEfZG0oOfpk9beWn3HUWtD+01KcCgXjnIHJQ==", "d50bf28d-f67d-49c6-8323-e53c03f4a15a" });

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsActive",
                value: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Houses");

            //migrationBuilder.UpdateData(
            //    table: "AspNetUsers",
            //    keyColumn: "Id",
            //    keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
            //    columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
            //    values: new object[] { "042e3c40-7324-41dd-9106-34fb5ccb5f3e", "AQAAAAEAACcQAAAAEJsu/YzwwWUeqB3GkfLmw11HiOqbAyZPjWPGUCzwQKQTOeh0fktN3fWB4ZJaOQe41A==", "dd52a8fd-36eb-430c-b5bf-20a28312fde0" });

            //migrationBuilder.UpdateData(
            //    table: "AspNetUsers",
            //    keyColumn: "Id",
            //    keyValue: "dea12856-c198-4129-b3f3-b893d8395082",
            //    columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
            //    values: new object[] { "ab2d4cc1-ed04-4fcf-995a-80f78d7942ed", "AQAAAAEAACcQAAAAEFA8iK0FMBr1Xfrxjgxn5aQ+2NZroo6BVO2OgNxrRfp9e0Q7vXpCoGzumxlQjHp+Cw==", "0f457eb2-7545-48ed-bd89-259c29cb6d7c" });
        }
    }
}
