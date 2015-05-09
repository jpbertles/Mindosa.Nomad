CREATE TABLE `nomad_history` (
  `migration_id` INT NOT NULL AUTO_INCREMENT,
  `migration_version` VARCHAR(8000) NOT NULL,
  `description` VARCHAR(8000) NOT NULL,
  `status` VARCHAR(50) NOT NULL,
  `hash_code` int, 
  `time_stamp` DATETIME NOT NULL,
  PRIMARY KEY (`migration_id`));
