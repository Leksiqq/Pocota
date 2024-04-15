using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServerImpl.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SetOfSauce",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id1 = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsVegan = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetOfSauce", x => new { x.Id, x.Id1 });
                });

            migrationBuilder.CreateTable(
                name: "SetOfTopping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Calories = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetOfTopping", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SetOfPizza",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SauceId = table.Column<int>(type: "int", nullable: true),
                    SauceId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetOfPizza", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SetOfPizza_SetOfSauce_SauceId_SauceId1",
                        columns: x => new { x.SauceId, x.SauceId1 },
                        principalTable: "SetOfSauce",
                        principalColumns: new[] { "Id", "Id1" });
                });

            migrationBuilder.CreateTable(
                name: "PizzaTopping",
                columns: table => new
                {
                    PizzasId = table.Column<int>(type: "int", nullable: false),
                    ToppingsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PizzaTopping", x => new { x.PizzasId, x.ToppingsId });
                    table.ForeignKey(
                        name: "FK_PizzaTopping_SetOfPizza_PizzasId",
                        column: x => x.PizzasId,
                        principalTable: "SetOfPizza",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PizzaTopping_SetOfTopping_ToppingsId",
                        column: x => x.ToppingsId,
                        principalTable: "SetOfTopping",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PizzaTopping_ToppingsId",
                table: "PizzaTopping",
                column: "ToppingsId");

            migrationBuilder.CreateIndex(
                name: "IX_SetOfPizza_SauceId_SauceId1",
                table: "SetOfPizza",
                columns: new[] { "SauceId", "SauceId1" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PizzaTopping");

            migrationBuilder.DropTable(
                name: "SetOfPizza");

            migrationBuilder.DropTable(
                name: "SetOfTopping");

            migrationBuilder.DropTable(
                name: "SetOfSauce");
        }
    }
}
