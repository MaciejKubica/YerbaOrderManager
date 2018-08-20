--
-- File generated with SQLiteStudio v3.1.1 on pon. sie 20 14:47:19 2018
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: Roles
DROP TABLE IF EXISTS Roles;
CREATE TABLE Roles (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL);
INSERT INTO Roles (Id, Name) VALUES (1, 'Administrator');
INSERT INTO Roles (Id, Name) VALUES (2, 'Supervisor');
INSERT INTO Roles (Id, Name) VALUES (3, 'StandardUser');
INSERT INTO Roles (Id, Name) VALUES (4, 'None');

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
