using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Commerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "commerce");

            migrationBuilder.CreateTable(
                name: "customers",
                schema: "commerce",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    middle_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    full_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    phone_number_value = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    phone_number_country_id = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    phone_number_country_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    phone_number_description = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    city_id = table.Column<int>(type: "integer", nullable: false),
                    address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    aggregate_id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    last_modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    version = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                schema: "commerce",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    brand = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    serial_id = table.Column<int>(type: "integer", nullable: true),
                    owner_id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true),
                    dealer_id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true),
                    device_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    panel_model = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    panel_serial_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    warranty_card_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    date_of_purchase = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    invoice_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    purchase_price = table.Column<decimal>(type: "numeric", nullable: true),
                    is_unrepairable = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    date_of_demand_for_compensation = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    demander_full_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    aggregate_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    last_modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    version = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "customer_order",
                schema: "commerce",
                columns: table => new
                {
                    customer_id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    order_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    centre_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_order", x => new { x.customer_id, x.order_id });
                    table.ForeignKey(
                        name: "FK_customer_order_customers_customer_id",
                        column: x => x.customer_id,
                        principalSchema: "commerce",
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "customer_product",
                schema: "commerce",
                columns: table => new
                {
                    customer_id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    product_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_product", x => new { x.customer_id, x.product_id });
                    table.ForeignKey(
                        name: "FK_customer_product_customers_customer_id",
                        column: x => x.customer_id,
                        principalSchema: "commerce",
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_customer_product_products_product_id",
                        column: x => x.product_id,
                        principalSchema: "commerce",
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_order",
                schema: "commerce",
                columns: table => new
                {
                    product_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    order_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    centre_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_order", x => new { x.product_id, x.order_id });
                    table.ForeignKey(
                        name: "FK_product_order_products_product_id",
                        column: x => x.product_id,
                        principalSchema: "commerce",
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_customer_product_product_id",
                schema: "commerce",
                table: "customer_product",
                column: "product_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customer_order",
                schema: "commerce");

            migrationBuilder.DropTable(
                name: "customer_product",
                schema: "commerce");

            migrationBuilder.DropTable(
                name: "product_order",
                schema: "commerce");

            migrationBuilder.DropTable(
                name: "customers",
                schema: "commerce");

            migrationBuilder.DropTable(
                name: "products",
                schema: "commerce");
        }
    }
}
