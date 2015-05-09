INSERT INTO nomad_history (migration_version, `description`, `status`, hash_code, time_stamp) 
	VALUES (@MigrationVersion, @Description, @Status, @HashCode, UTC_TIMESTAMP())