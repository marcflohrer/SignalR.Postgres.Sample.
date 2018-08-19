using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace StockTickR.Migrations
{
    public partial class seed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Symbol = table.Column<string>(nullable: true),
                    DayOpen = table.Column<decimal>(nullable: false),
                    DayLow = table.Column<decimal>(nullable: false),
                    DayHigh = table.Column<decimal>(nullable: false),
                    LastChange = table.Column<decimal>(nullable: false),
                    Price = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Stocks",
                columns: new[] { "Id", "DayHigh", "DayLow", "DayOpen", "LastChange", "Price", "Symbol" },
                values: new object[] { 1, 75.12m, 75.12m, 75.12m, 0m, 75.12m, "MSFT" });

            migrationBuilder.InsertData(
                table: "Stocks",
                columns: new[] { "Id", "DayHigh", "DayLow", "DayOpen", "LastChange", "Price", "Symbol" },
                values: new object[] { 2, 158.44m, 158.44m, 158.44m, 0m, 158.44m, "AAPL" });

            migrationBuilder.InsertData(
                table: "Stocks",
                columns: new[] { "Id", "DayHigh", "DayLow", "DayOpen", "LastChange", "Price", "Symbol" },
                values: new object[] { 3, 1200.96m, 1200.96m, 1200.96m, 0m, 1200.96m, "GOOG" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stocks");
        }
    }
}
