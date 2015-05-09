IF NOT EXISTS (SELECT * FROM dbo.Nomad_History)
BEGIN
    INSERT INTO dbo.Nomad_History (MigrationVersion, [Description], [Status], HashCode, [TimeStamp]) 
	VALUES ('0', 'Baseline', 'Baseline', {0}, getutcdate())
END