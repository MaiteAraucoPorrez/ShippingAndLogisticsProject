using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingAndLogisticsManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdentityDocument = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LicenseCategory = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LicenseIssueDate = table.Column<DateTime>(type: "date", nullable: false),
                    LicenseExpiryDate = table.Column<DateTime>(type: "date", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AlternativePhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: false),
                    HireDate = table.Column<DateTime>(type: "date", nullable: false),
                    ContractEndDate = table.Column<DateTime>(type: "date", nullable: true),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CurrentVehicleId = table.Column<int>(type: "int", nullable: true),
                    AverageRating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    TotalDeliveries = table.Column<int>(type: "int", nullable: false),
                    EmergencyContactName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmergencyContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BloodType = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Origin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DistanceKm = table.Column<double>(type: "float", nullable: false),
                    BaseCost = table.Column<double>(type: "float", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Security",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Security", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MaxCapacityM3 = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CurrentCapacityM3 = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OperatingHours = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ManagerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(10,7)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(10,7)", nullable: true),
                    OpeningDate = table.Column<DateTime>(type: "date", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Zone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Alias = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(10,7)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(10,7)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Shipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShippingDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    State = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    RouteId = table.Column<int>(type: "int", nullable: false),
                    TotalCost = table.Column<double>(type: "float", nullable: false),
                    TrackingNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RouteId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shipments_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Shipments_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipments_Routes_RouteId1",
                        column: x => x.RouteId1,
                        principalTable: "Routes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlateNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaxWeightCapacityKg = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    MaxVolumeCapacityM3 = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CurrentWeightKg = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    CurrentVolumeM3 = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LastMaintenanceDate = table.Column<DateTime>(type: "date", nullable: true),
                    NextMaintenanceDate = table.Column<DateTime>(type: "date", nullable: true),
                    CurrentMileage = table.Column<int>(type: "int", nullable: false),
                    LastMaintenanceMileage = table.Column<int>(type: "int", nullable: true),
                    FuelType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FuelConsumptionPer100Km = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    VIN = table.Column<string>(type: "nvarchar(17)", maxLength: 17, nullable: true),
                    InsurancePolicyNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InsuranceExpiryDate = table.Column<DateTime>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BaseWarehouseId = table.Column<int>(type: "int", nullable: true),
                    AssignedDriverId = table.Column<int>(type: "int", nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "date", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_Drivers_AssignedDriverId",
                        column: x => x.AssignedDriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Vehicles_Warehouses_BaseWarehouseId",
                        column: x => x.BaseWarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    ShipmentId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Packages_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentWarehouses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShipmentId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ExitDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ReceivedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DispatchedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StorageLocation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentWarehouses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipmentWarehouses_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShipmentWarehouses_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_Customer_Type_Default",
                table: "Addresses",
                columns: new[] { "CustomerId", "Type", "IsDefault" });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_City",
                table: "Addresses",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CustomerId",
                table: "Addresses",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_Department",
                table: "Addresses",
                column: "Department");

            migrationBuilder.CreateIndex(
                name: "IX_Driver_Active_Status",
                table: "Drivers",
                columns: new[] { "IsActive", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Driver_Email",
                table: "Drivers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Driver_IdentityDocument_Unique",
                table: "Drivers",
                column: "IdentityDocument",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Driver_LicenseNumber_Unique",
                table: "Drivers",
                column: "LicenseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_IsActive",
                table: "Drivers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_LicenseExpiryDate",
                table: "Drivers",
                column: "LicenseExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_Status",
                table: "Drivers",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_ShipmentId",
                table: "Packages",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_CustomerId",
                table: "Shipments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_RouteId",
                table: "Shipments",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_RouteId1",
                table: "Shipments",
                column: "RouteId1");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentWarehouses_ShipmentId",
                table: "ShipmentWarehouses",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentWarehouses_ShipmentId_WarehouseId",
                table: "ShipmentWarehouses",
                columns: new[] { "ShipmentId", "WarehouseId" });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentWarehouses_Status",
                table: "ShipmentWarehouses",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentWarehouses_WarehouseId",
                table: "ShipmentWarehouses",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_Active_Status",
                table: "Vehicles",
                columns: new[] { "IsActive", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_PlateNumber_Unique",
                table: "Vehicles",
                column: "PlateNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_VIN_Unique",
                table: "Vehicles",
                column: "VIN",
                unique: true,
                filter: "[VIN] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_AssignedDriverId",
                table: "Vehicles",
                column: "AssignedDriverId",
                unique: true,
                filter: "[AssignedDriverId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_BaseWarehouseId",
                table: "Vehicles",
                column: "BaseWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_IsActive",
                table: "Vehicles",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_NextMaintenanceDate",
                table: "Vehicles",
                column: "NextMaintenanceDate");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Status",
                table: "Vehicles",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Type",
                table: "Vehicles",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_Code_Unique",
                table: "Warehouses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_City",
                table: "Warehouses",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_Department",
                table: "Warehouses",
                column: "Department");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_IsActive",
                table: "Warehouses",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_Type",
                table: "Warehouses",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Packages");

            migrationBuilder.DropTable(
                name: "Security");

            migrationBuilder.DropTable(
                name: "ShipmentWarehouses");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Shipments");

            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Routes");
        }
    }
}
