# Learning SQLite

### Creating new table for Doctors

```SQL
CREATE TABLE Doctors (
  DoctorID INTEGER PRIMARY KEY AUTOINCREMENT,
  Name TEXT NOT NULL,
  Surname Text NOT NULL,
  Multiplier DECIMAL NOT NULL
);
```

### Inserting some data into my new table

```SQL
INSERT INTO Doctors (Name, Surname, Multiplier)
VALUES
  ('Anna', 'Apse', 2),
  ('Oskars', 'Andersons', 0.5),
  ('Jenifere', 'Pottere', 3.2);
```

### Deleting rows (records) from the table
```SQL
  DELETE FROM Doctors WHERE DoctorID > 10;
```

### Adding new calculated column to the query
```SQL
  SELECT name, surname, multiplier, (multiplier * 100) AS NewMultiplier
  FROM Doctors;
```
