CREATE TABLE TestTable (id INT NOT NULL, name VARCHAR(50))

GO

CREATE PROCEDURE TestTable_Select_ById
(
	@id int
)
AS
BEGIN
	SELECT id, name FROM TestTable WHERE id = @id
END