IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'Nomad_History')
BEGIN
	SELECT 
		HashCode, 
		[Description], 
		MigrationVersion, 
		[Status], 
		[TimeStamp], 
		MigrationId 
	FROM Nomad_History
END