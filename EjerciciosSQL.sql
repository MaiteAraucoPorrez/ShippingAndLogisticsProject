Use DbSocialMedia;

--Usuarios activos que nunca han comentado
SELECT u.FirstName, u.LastName, u.Email
FROM [User] u
LEFT JOIN Comment c ON u.Id = c.UserId
WHERE c.Id IS NULL 
AND u.IsActive = 1;


--Comentarios realizados hace 3 meses por usuarios mayores de 25 años
SELECT 
c.Id AS IdComment,
c.Description AS CommentDescription,
u.FirstName, u.LastName,
DATEDIFF(YEAR, u.DateOfBirth, GETDATE()) AS Edad
FROM Comment c
JOIN [User] u ON c.UserId = u.Id
WHERE c.Date >= DATEADD(MONTH, -3, GETDATE())
AND DATEDIFF(YEAR, u.DateOfBirth, GETDATE()) > 25;


--Posts sin comentarios de usuarios activos
SELECT 
p.Id AS IdPost,
p.Description AS PostDescription,
p.Date AS PostDate
FROM Post p
LEFT JOIN Comment c ON p.Id = c.PostId
LEFT JOIN [User] u ON c.UserId = u.Id AND u.IsActive = 1
WHERE c.Id IS NULL OR u.Id IS NULL;


--Usuarios que han comentado en posts de al menos 3 usuarios diferentes
SELECT u.FirstName, u.LastName,
COUNT(DISTINCT p.UserId) AS UsuariosDiferentes
FROM Comment c
JOIN Post p ON c.PostId = p.Id
JOIN [User] u ON c.UserId = u.Id
GROUP BY u.FirstName, u.LastName
HAVING COUNT(DISTINCT p.UserId) >= 3;


--Posts con comentarios de usuarios menores de 18 años
SELECT 
p.Id AS IdPost,
p.Description AS PostDescription,
COUNT(c.Id) AS ComentariosMenores
FROM Post p
JOIN Comment c ON p.Id = c.PostId
JOIN [User] u ON c.UserId = u.Id
WHERE DATEDIFF(YEAR, u.DateOfBirth, GETDATE()) < 18
GROUP BY p.Id, p.Description;


--Densidad de comentarios por día de la semana
SELECT 
DATENAME(WEEKDAY, c.Date) AS DiaSemana,
COUNT(c.Id) AS TotalComentarios,
COUNT(DISTINCT c.UserId) AS UsuariosUnicos
FROM Comment c
GROUP BY DATENAME(WEEKDAY, c.Date)
ORDER BY TotalComentarios DESC;


--Crecimiento mensual de comentarios
WITH ComentariosPorMes AS (
SELECT 
YEAR(c.Date) AS Año,
MONTH(c.Date) AS Mes,
COUNT(c.Id) AS TotalComentarios
FROM Comment c
GROUP BY YEAR(c.Date), MONTH(c.Date))

SELECT Año, Mes, TotalComentarios,
LAG(TotalComentarios) OVER (ORDER BY Año, Mes) AS MesAnterior,
ROUND(
(CAST(TotalComentarios AS FLOAT) - LAG(TotalComentarios) OVER (ORDER BY Año, Mes)) 
/ NULLIF(LAG(TotalComentarios) OVER (ORDER BY Año, Mes), 0) * 100, 2) AS CrecimientoPorcentual
FROM ComentariosPorMes
ORDER BY Año, Mes;

