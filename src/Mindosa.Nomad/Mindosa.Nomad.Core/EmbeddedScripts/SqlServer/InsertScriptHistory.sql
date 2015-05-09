INSERT INTO dbo.Nomad_History (MigrationVersion, [Description], [Status], HashCode, [TimeStamp]) 
	VALUES (@MigrationVersion, @Description, @Status, @HashCode, getutcdate())