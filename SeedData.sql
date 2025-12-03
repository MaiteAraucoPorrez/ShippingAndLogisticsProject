
-- CUSTOMERS (Clientes)
SET IDENTITY_INSERT Customers ON;
INSERT INTO Customers (Id, Name, Email, Phone) VALUES
(1, 'Juan Perez', 'juan.perez@example.com', '70010001'),
(2, 'Maria Gomez', 'maria.gomez@example.com', '70010002'),
(3, 'Carlos Sanchez', 'carlos.sanchez@example.com', '70010003'),
(4, 'Laura Fernandez', 'laura.fernandez@example.com', '70010004'),
(5, 'Pedro Ramirez', 'pedro.ramirez@example.com', '70010005'),
(6, 'Ana Torres', 'ana.torres@example.com', '70010006'),
(7, 'Diego Lopez', 'diego.lopez@example.com', '70010007'),
(8, 'Sofia Morales', 'sofia.morales@example.com', '70010008'),
(9, 'Jorge Castillo', 'jorge.castillo@example.com', '70010009'),
(10, 'Camila Herrera', 'camila.herrera@example.com', '70010010');
SET IDENTITY_INSERT Customers OFF;
GO

-- ROUTES (Rutas)
SET IDENTITY_INSERT Routes ON;
INSERT INTO Routes (Id, Origin, Destination, DistanceKm, BaseCost, IsActive) VALUES
(1, 'La Paz', 'Santa Cruz', 850, 150.00, 1),
(2, 'Cochabamba', 'La Paz', 380, 80.00, 1),
(3, 'Oruro', 'Potosi', 220, 60.00, 1),
(4, 'Sucre', 'Tarija', 450, 100.00, 1),
(5, 'Santa Cruz', 'Trinidad', 410, 90.00, 1),
(6, 'Cochabamba', 'Santa Cruz', 470, 110.00, 1),
(7, 'La Paz', 'Oruro', 230, 70.00, 1),
(8, 'Tarija', 'Santa Cruz', 600, 130.00, 0),
(9, 'Potosi', 'Sucre', 150, 50.00, 1),
(10, 'Santa Cruz', 'La Paz', 850, 150.00, 1);
SET IDENTITY_INSERT Routes OFF;
GO

-- WAREHOUSES (Almacenes)
SET IDENTITY_INSERT Warehouses ON;
INSERT INTO Warehouses (Id, Name, Code, Address, City, Department, Phone, Email, MaxCapacityM3, CurrentCapacityM3, IsActive, Type, OperatingHours, ManagerName) VALUES
(1, 'Almacén Central La Paz', 'WH-LP-001', 'Av. Buenos Aires Km 7.5, Zona Industrial', 'La Paz', 'La Paz', '2-2234567', 'almacen.lapaz@empresa.com', 2000.0, 650.5, 1, 1, 'Lunes a Viernes 8:00-18:00, Sábados 8:00-13:00', 'Roberto Gutierrez'),
(2, 'Almacén Regional Cochabamba', 'WH-CB-001', 'Av. Petrolera Km 5, Parque Industrial', 'Cochabamba', 'Cochabamba', '4-4445566', 'almacen.cbba@empresa.com', 1500.0, 450.0, 1, 2, 'Lunes a Sábado 7:00-19:00', 'Carmen Flores'),
(3, 'Almacén Regional Santa Cruz', 'WH-SC-001', 'Doble Vía La Guardia Km 8', 'Santa Cruz', 'Santa Cruz', '3-3556677', 'almacen.scz@empresa.com', 2500.0, 890.0, 1, 2, 'Lunes a Sábado 7:00-20:00', 'Miguel Vargas'),
(4, 'Almacén Local Sucre', 'WH-SU-001', 'Carretera a Potosí Km 2', 'Sucre', 'Chuquisaca', '4-6667788', 'almacen.sucre@empresa.com', 800.0, 220.0, 1, 3, 'Lunes a Viernes 8:00-17:00', 'Patricia Rojas'),
(5, 'Almacén Local Oruro', 'WH-OR-001', 'Av. 6 de Octubre s/n, Zona Industrial', 'Oruro', 'Oruro', '2-5778899', 'almacen.oruro@empresa.com', 1000.0, 340.0, 1, 3, 'Lunes a Viernes 8:00-18:00', 'Fernando Quispe');
SET IDENTITY_INSERT Warehouses OFF;
GO

-- DRIVERS (Conductores)
SET IDENTITY_INSERT Drivers ON;
INSERT INTO Drivers (Id, FullName, LicenseNumber, Phone, Email, Address, City, DateOfBirth, IsActive, Status, CurrentVehicleId, TotalDeliveries) VALUES
(1, 'Luis Alberto Mamani', 'LIC-2020-001234', '71234567', 'luis.mamani@empresa.com', 'Calle Los Pinos #456', 'La Paz', '1985-05-20', 1, 1, NULL, 250),
(2, 'Rosa Elena Condori', 'LIC-2019-005678', '72345678', 'rosa.condori@empresa.com', 'Av. América #789', 'Cochabamba', '1990-08-15', 1, 2, NULL, 320),
(3, 'Marco Antonio Quispe', 'LIC-2021-009012', '73456789', 'marco.quispe@empresa.com', 'Calle Libertad #234', 'Santa Cruz', '1988-12-10', 1, 1, NULL, 410),
(4, 'Sandra Patricia Flores', 'LIC-2020-003456', '74567890', 'sandra.flores@empresa.com', 'Calle Junín #567', 'Oruro', '1992-03-25', 1, 3, NULL, 180),
(5, 'Javier Raul Mendoza', 'LIC-2018-007890', '75678901', 'javier.mendoza@empresa.com', 'Av. Simón Bolívar #890', 'Potosí', '1987-09-18', 1, 1, NULL, 520);
SET IDENTITY_INSERT Drivers OFF;
GO

-- VEHICLES (Vehículos)
SET IDENTITY_INSERT Vehicles ON;
INSERT INTO Vehicles (Id, PlateNumber, Type, MaxWeightCapacityKg, MaxVolumeCapacityM3, CurrentWeightKg, CurrentVolumeM3, Status, VIN, IsActive, BaseWarehouseId, AssignedDriverId) VALUES
(1, '1234-ABC', 3, 3500.0, 15.5, 1200.0, 8.5, 2, '1HGBH41JXMN109186', 1, 1, 1),
(2, '2345-BCD', 2, 1800.0, 10.0, 650.0, 5.2, 1, '2HGBH41JXMN109187', 1, 2, 2),
(3, '3456-CDE', 4, 5000.0, 25.0, 2100.0, 12.0, 2, '3HGBH41JXMN109188', 1, 3, 3),
(4, '4567-DEF', 2, 1500.0, 8.0, 0.0, 0.0, 3, '4HGBH41JXMN109189', 1, 4, NULL),
(5, '5678-EFG', 2, 1600.0, 9.5, 580.0, 4.8, 1, '5HGBH41JXMN109190', 1, 5, 5),
(6, '6789-FGH', 1, 200.0, 0.5, 15.0, 0.2, 1, '6HGBH41JXMN109191', 1, 1, NULL);
SET IDENTITY_INSERT Vehicles OFF;
GO

-- Actualizacion CurrentVehicleId en Drivers
UPDATE Drivers SET CurrentVehicleId = 1 WHERE Id = 1;
UPDATE Drivers SET CurrentVehicleId = 2 WHERE Id = 2;
UPDATE Drivers SET CurrentVehicleId = 3 WHERE Id = 3;
UPDATE Drivers SET CurrentVehicleId = 5 WHERE Id = 5;
GO

-- ADDRESSES (Direcciones)
SET IDENTITY_INSERT Addresses ON;
INSERT INTO Addresses (Id, CustomerId, Street, City, Department, Zone, Type, IsDefault, Reference, ContactName, ContactPhone, IsActive) VALUES
-- Cliente 1 (Juan Perez)
(1, 1, 'Av. 6 de Agosto #2170, Edificio Torres del Poeta, Piso 5', 'La Paz', 'La Paz', 'Sopocachi', 1, 1, 'Cerca del Monoblock Central', 'Juan Perez', '70010001', 1),
(2, 1, 'Calle Comercio #456, Zona Norte', 'La Paz', 'La Paz', 'Zona Norte', 2, 1, 'Al lado del mercado', 'Juan Perez', '70010001', 1),
-- Cliente 2 (Maria Gomez)
(3, 2, 'Av. Blanco Galindo Km 4.5', 'Cochabamba', 'Cochabamba', 'Quillacollo', 1, 1, 'Frente a la estación de servicio', 'Maria Gomez', '70010002', 1),
(4, 2, 'Calle Esteban Arce #890', 'Cochabamba', 'Cochabamba', 'Centro', 2, 1, 'Edificio azul, 2do piso', 'Maria Gomez', '70010002', 1),
-- Cliente 3 (Carlos Sanchez)
(5, 3, 'Av. Cristo Redentor #1234', 'Santa Cruz', 'Santa Cruz', 'Equipetrol', 1, 1, 'Casa con portón negro', 'Carlos Sanchez', '70010003', 1),
-- Cliente 4 (Laura Fernandez)
(6, 4, 'Calle Dalence #567', 'Sucre', 'Chuquisaca', 'Centro Histórico', 2, 1, 'Cerca de la Plaza 25 de Mayo', 'Laura Fernandez', '70010004', 1),
-- Cliente 5 (Pedro Ramirez)
(7, 5, 'Av. Cincuentenario #2345', 'Oruro', 'Oruro', 'Norte', 1, 1, 'Junto al hospital', 'Pedro Ramirez', '70010005', 1),
-- Cliente 6 (Ana Torres)
(8, 6, 'Calle Aroma #678', 'La Paz', 'La Paz', 'Miraflores', 2, 1, 'Frente al parque', 'Ana Torres', '70010006', 1),
-- Cliente 7 (Diego Lopez)
(9, 7, 'Av. Heroínas #890', 'Cochabamba', 'Cochabamba', 'Recoleta', 1, 1, 'Casa amarilla', 'Diego Lopez', '70010007', 1),
-- Cliente 8 (Sofia Morales)
(10, 8, 'Calle 21 de Mayo #1234', 'Santa Cruz', 'Santa Cruz', 'Centro', 2, 1, 'Edificio El Fuerte', 'Sofia Morales', '70010008', 1);
SET IDENTITY_INSERT Addresses OFF;
GO

-- SHIPMENTS (Envíos)
SET IDENTITY_INSERT Shipments ON;
INSERT INTO Shipments (Id, ShippingDate, State, CustomerId, RouteId, TotalCost, TrackingNumber) VALUES
(1, '2025-09-01', 'Pending', 1, 1, 500.00, 'CP-10001'),
(2, '2025-09-02', 'In Transit', 2, 2, 200.00, 'CP-10002'),
(3, '2025-09-03', 'Delivered', 3, 3, 150.00, 'CP-10003'),
(4, '2025-09-04', 'Pending', 4, 4, 600.00, 'CP-10004'),
(5, '2025-09-05', 'Delivered', 5, 5, 350.00, 'CP-10005'),
(6, '2025-09-06', 'In Transit', 6, 6, 290.00, 'CP-10006'),
(7, '2025-09-07', 'Pending', 7, 7, 220.00, 'CP-10007'),
(8, '2025-09-08', 'Delivered', 8, 8, 530.00, 'CP-10008'),
(9, '2025-09-09', 'In Transit', 9, 9, 140.00, 'CP-10009'),
(10, '2025-09-10', 'Pending', 10, 10, 210.00, 'CP-10010'),
(11, '2025-09-11', 'Delivered', 1, 2, 260.00, 'CP-10011'),
(12, '2025-09-12', 'Pending', 2, 3, 130.00, 'CP-10012'),
(13, '2025-09-13', 'In Transit', 3, 4, 720.00, 'CP-10013'),
(14, '2025-09-14', 'Delivered', 4, 5, 390.00, 'CP-10014'),
(15, '2025-09-15', 'Pending', 5, 6, 410.00, 'CP-10015');
SET IDENTITY_INSERT Shipments OFF;
GO

-- PACKAGES (Paquetes)
SET IDENTITY_INSERT Packages ON;
INSERT INTO Packages (Id, Description, Weight, ShipmentId, Price) VALUES
(1, 'Electrodomesticos', 12.5, 1, 350.00),
(2, 'Ropa', 5.2, 2, 120.00),
(3, 'Libros', 3.0, 3, 90.00),
(4, 'Muebles', 20.0, 4, 500.00),
(5, 'Alimentos no perecederos', 15.0, 5, 250.00),
(6, 'Herramientas', 7.8, 6, 180.00),
(7, 'Juguetes', 4.3, 7, 75.00),
(8, 'Equipos electronicos', 8.9, 8, 400.00),
(9, 'Cosmeticos', 2.5, 9, 60.00),
(10, 'Medicamentos', 1.2, 10, 45.00),
(11, 'Accesorios para auto', 6.7, 11, 210.00),
(12, 'Ropa deportiva', 5.5, 12, 130.00),
(13, 'Instrumentos musicales', 10.4, 13, 650.00),
(14, 'Alimentos frescos', 18.3, 14, 300.00),
(15, 'Articulos de oficina', 4.6, 15, 95.00),
(16, 'Laptop HP', 2.8, 1, 150.00),
(17, 'Monitor Samsung 24"', 4.5, 2, 80.00),
(18, 'Zapatillas Nike', 1.2, 6, 110.00),
(19, 'Perfumes importados', 0.8, 9, 80.00),
(20, 'Repuestos automotrices', 12.0, 13, 70.00);
SET IDENTITY_INSERT Packages OFF;
GO

-- SHIPMENT_WAREHOUSES (Tracking)
SET IDENTITY_INSERT ShipmentWarehouses ON;
INSERT INTO ShipmentWarehouses (Id, ShipmentId, WarehouseId, EntryDate, ExitDate, Status, ReceivedBy, DispatchedBy, StorageLocation) VALUES
-- Envio 1: La Paz -> Santa Cruz (Ruta 1)
(1, 1, 1, '2025-09-01 08:00:00', '2025-09-01 14:00:00', 4, 'jperez', 'mrodriguez', 'Estante A-12, Nivel 3'),
(2, 1, 3, '2025-09-02 09:00:00', NULL, 2, 'agarcia', NULL, 'Zona B-05'),
-- Envio 2: Cochabamba -> La Paz (Ruta 2) - En transito
(3, 2, 2, '2025-09-02 07:30:00', '2025-09-02 16:00:00', 4, 'lmartinez', 'plopez', 'Estante C-08'),
(4, 2, 1, '2025-09-03 10:00:00', NULL, 3, 'jperez', NULL, 'Zona A-15'),
-- Envio 3: Oruro -> Potosi (Ruta 3) - Entregado
(5, 3, 5, '2025-09-03 08:00:00', '2025-09-03 12:00:00', 4, 'fquispe', 'rsanchez', 'Área D-02'),
-- Envio 5: Santa Cruz -> Trinidad (Ruta 5) - Entregado
(6, 5, 3, '2025-09-05 09:00:00', '2025-09-05 15:00:00', 4, 'agarcia', 'lcastro', 'Zona E-10'),
-- Envio 6: Cochabamba -> Santa Cruz (Ruta 6) - En transito
(7, 6, 2, '2025-09-06 08:30:00', '2025-09-06 17:00:00', 4, 'lmartinez', 'plopez', 'Estante F-03'),
(8, 6, 3, '2025-09-07 11:00:00', NULL, 2, 'agarcia', NULL, 'Zona G-07'),
-- Envio 8: Tarija -> Santa Cruz (Ruta 8) - Entregado
(9, 8, 4, '2025-09-08 07:00:00', '2025-09-08 13:00:00', 4, 'projas', 'mvargas', 'Área H-01'),
-- Envio 9: Potosi -> Sucre (Ruta 9) - En transito
(10, 9, 4, '2025-09-09 09:30:00', NULL, 3, 'projas', NULL, 'Zona I-04'),
-- Envio 13: En transito entre almacenes
(11, 13, 2, '2025-09-13 08:00:00', '2025-09-13 15:00:00', 4, 'lmartinez', 'plopez', 'Estante J-02'),
(12, 13, 3, '2025-09-14 10:00:00', NULL, 2, 'agarcia', NULL, 'Zona K-08');
SET IDENTITY_INSERT ShipmentWarehouses OFF;
GO

-- SECURITY (Usuarios)
SET IDENTITY_INSERT Security ON;
INSERT INTO Security (Id, Login, Password, Name, Role) VALUES
(1, 'admin', 'Admin123!', 'Administrador Sistema', 0),
(2, 'empleado1', 'Emp123!', 'Carlos Ramirez', 1),
(3, 'cliente1', 'Client123!', 'Maria Lopez', 2),
(4, 'empleado2', 'Emp123!', 'Juan Torres', 1),
(5, 'cliente2', 'Client123!', 'Ana Gutierrez', 2),
(6, 'jperez', 'Emp123!', 'Jose Perez', 1),
(7, 'agarcia', 'Emp123!', 'Andrea Garcia', 1);
SET IDENTITY_INSERT Security OFF;
GO