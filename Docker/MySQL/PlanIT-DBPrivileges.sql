# create user
CREATE USER IF NOT EXISTS 'planit-user'@'localhost' IDENTIFIED BY '5ecret-plan1t';
CREATE USER IF NOT EXISTS 'planit-user'@'%' IDENTIFIED BY '5ecret-plan1t';
 
GRANT ALL privileges ON planit_db.* TO 'planit-user'@'%';
GRANT ALL privileges ON planit_db.* TO 'planit-user'@'localhost';
FLUSH PRIVILEGES;