
INSERT INTO Customers (Name, Email, Phone) VALUES
('Juan Perez', 'juan.perez@example.com', '70010001'),
('Maria Gomez', 'maria.gomez@example.com', '70010002'),
('Carlos Sanchez', 'carlos.sanchez@example.com', '70010003'),
('Laura Fernandez', 'laura.fernandez@example.com', '70010004'),
('Pedro Ramirez', 'pedro.ramirez@example.com', '70010005'),
('Ana Torres', 'ana.torres@example.com', '70010006'),
('Diego Lopez', 'diego.lopez@example.com', '70010007'),
('Sofia Morales', 'sofia.morales@example.com', '70010008'),
('Jorge Castillo', 'jorge.castillo@example.com', '70010009'),
('Camila Herrera', 'camila.herrera@example.com', '70010010');


INSERT INTO Routes (Origin, Destination, DistanceKm, BaseCost, IsActive) VALUES
('La Paz', 'Santa Cruz', 850, 150.00, 1),
('Cochabamba', 'La Paz', 380, 80.00, 1),
('Oruro', 'Potosi', 220, 60.00, 1),
('Sucre', 'Tarija', 450, 100.00, 1),
('Santa Cruz', 'Trinidad', 410, 90.00, 1),
('Cochabamba', 'Santa Cruz', 470, 110.00, 1),
('La Paz', 'Oruro', 230, 70.00, 1),
('Tarija', 'Santa Cruz', 600, 130.00, 0),
('Potosi', 'Sucre', 150, 50.00, 1),
('Santa Cruz', 'La Paz', 850, 150.00, 1);


INSERT INTO Shipments (ShippingDate, State, CustomerId, RouteId, TotalCost, TrackingNumber) VALUES
('2025-09-01', 'Pending', 1, 1, 500.00, 'CP-10001'),
('2025-09-02', 'In Transit', 2, 2, 200.00, 'CP-10002'),
('2025-09-03', 'Delivered', 3, 3, 150.00, 'CP-10003'),
('2025-09-04', 'Pending', 4, 4, 600.00, 'CP-10004'),
('2025-09-05', 'Delivered', 5, 5, 350.00, 'CP-10005'),
('2025-09-06', 'In Transit', 6, 6, 290.00, 'CP-10006'),
('2025-09-07', 'Pending', 7, 7, 220.00, 'CP-10007'),
('2025-09-08', 'Delivered', 8, 8, 530.00, 'CP-10008'),
('2025-09-09', 'In Transit', 9, 9, 140.00, 'CP-10009'),
('2025-09-10', 'Pending', 10, 10, 210.00, 'CP-10010'),
('2025-09-11', 'Delivered', 1, 2, 260.00, 'CP-10011'),
('2025-09-12', 'Pending', 2, 3, 130.00, 'CP-10012'),
('2025-09-13', 'In Transit', 3, 4, 720.00, 'CP-10013'),
('2025-09-14', 'Delivered', 4, 5, 390.00, 'CP-10014'),
('2025-09-15', 'Pending', 5, 6, 410.00, 'CP-10015');


INSERT INTO Packages (Description, Weight, ShipmentId, Price) VALUES
('Electrodomesticos', 12.5, 1, 350.00),
('Ropa', 5.2, 2, 120.00),
('Libros', 3.0, 3, 90.00),
('Muebles', 20.0, 4, 500.00),
('Alimentos no perecederos', 15.0, 5, 250.00),
('Herramientas', 7.8, 6, 180.00),
('Juguetes', 4.3, 7, 75.00),
('Equipos electronicos', 8.9, 8, 400.00),
('Cosmeticos', 2.5, 9, 60.00),
('Medicamentos', 1.2, 10, 45.00),
('Accesorios para auto', 6.7, 11, 210.00),
('Ropa deportiva', 5.5, 12, 130.00),
('Instrumentos musicales', 10.4, 13, 650.00),
('Alimentos frescos', 18.3, 14, 300.00),
('Articulos de oficina', 4.6, 15, 95.00);

SELECT * FROM Customers;
SELECT * FROM Routes;
SELECT * FROM Shipments;
SELECT * FROM Packages;