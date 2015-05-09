CREATE TABLE TestTable (id INT NOT NULL, name VARCHAR(50));

CREATE PROCEDURE `TestTable_Select_ById` (in my_id int)
BEGIN
	select id, name
    from TestTable
    Where TestTable.id = my_id;
END;
