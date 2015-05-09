IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'Nomad_History')
BEGIN
    CREATE TABLE dbo.Nomad_History (
		MigrationId INT NOT NULL PRIMARY KEY IDENTITY, 
		MigrationVersion varchar(max) not null, 
		[Description] varchar(max) not null, 
		[Status] varchar(50) not null, 
		HashCode int, 
		[TimeStamp] DateTime not NULL
	)
END