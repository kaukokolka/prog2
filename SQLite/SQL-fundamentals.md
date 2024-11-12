# Learning SQLite

### Creating new table for Doctors

```SQL
CREATE TABLE Doctors (
	DoctorID INTEGER PRIMARY KEY AUTOINCREMENT,
  	Name TEXT NOT NULL,
  	Surname TEXT NOT NULL,
  	Multiplier DECIMAL NOT NULL
);
```
### Inserting data into new table

```SQL
INSERT INTO Doctors (Name, Surname, Multiplier)
VALUES
	('Anna', 'Apse', 2),
	('Oskars', 'Andersons', 0.5),
	('Jenifere', 'Pottere', 3.2);
```
