using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eventEaseBookingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerColumnsToBookings : Migration
    {        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if columns exist before adding them
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Bookings' AND COLUMN_NAME = 'CustomerName')
                BEGIN
                    ALTER TABLE [Bookings] ADD [CustomerName] nvarchar(100) NOT NULL DEFAULT '';
                END
            ");
            
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Bookings' AND COLUMN_NAME = 'CustomerEmail')
                BEGIN
                    ALTER TABLE [Bookings] ADD [CustomerEmail] nvarchar(100) NOT NULL DEFAULT '';
                END
            ");
            
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Bookings' AND COLUMN_NAME = 'CustomerPhone')
                BEGIN
                    ALTER TABLE [Bookings] ADD [CustomerPhone] nvarchar(15) NULL;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerEmail",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerPhone",
                table: "Bookings");
        }
    }
}
