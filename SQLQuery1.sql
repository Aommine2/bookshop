﻿CREATE TABLE messages (
 id int NOT NULL PRIMARY KEY IDENTITY,
 firstname VARCHAR (100) NOT NULL,
 lastname VARCHAR (100) NOT NULL,
 email VARCHAR (150) NOT NULL,
 phone VARCHAR (20) NOT NULL,
 subject VARCHAR (255) NOT NULL,
 message TEXT NOT NULL,
 create_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
 );