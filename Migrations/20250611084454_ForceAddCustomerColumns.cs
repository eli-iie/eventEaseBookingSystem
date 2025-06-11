using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eventEaseBookingSystem.Migrations
{
    /// <inheritdoc />
    public partial class ForceAddCustomerColumns : Migration
    {        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add customer columns to Bookings table using raw SQL
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
        }        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove customer columns from Bookings table
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Bookings' AND COLUMN_NAME = 'CustomerPhone')
                BEGIN
                    ALTER TABLE [Bookings] DROP COLUMN [CustomerPhone];
                END
            ");
            
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Bookings' AND COLUMN_NAME = 'CustomerEmail')
                BEGIN
                    ALTER TABLE [Bookings] DROP COLUMN [CustomerEmail];
                END
            ");
            
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Bookings' AND COLUMN_NAME = 'CustomerName')
                BEGIN
                    ALTER TABLE [Bookings] DROP COLUMN [CustomerName];
                END
            ");
        }
    }
}
