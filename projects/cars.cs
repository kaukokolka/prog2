using Microsoft.Data.Sqlite;
using System;

public class Program
{
	public static void Main(string[] args)
	{
		// Init app
		var app = new App("Data Source=mydatabase.db;");
		app.NewCar(1, "Model Y", 15.0f, 0.7f);
		app.NewClient(1, "Alice", "Smith", "alice.smith@example.com");
		app.Rent(1, 1, "2024-12-20 15:30:00"); 		// Start a rental for client1 with car1
    		app.EndRental(1, 120);  		// End rental 1 with 120 km
	}
}

public class App
{
	private string connectionString;
	private int rentalCount;
	public App(string connectionString) //constructor, init by providing source e.g. "Data Source=pizzas.db;"
	{
		this.connectionString = connectionString;
		rentalCount = 0; //rental counter for id display
		CreateTable();
	}

	private void CreateTable()
	{
		try
		{
			using (var connection = new SqliteConnection(connectionString))
			{
				connection.Open();
				using (var command = new SqliteCommand("PRAGMA foreign_keys = ON;", connection))
				{
					command.ExecuteNonQuery();
				}

				var createTableCmd = connection.CreateCommand();
				createTableCmd.CommandText = @"
						CREATE TABLE IF NOT EXISTS Cars (
							id INTEGER PRIMARY KEY,
							model TEXT NOT NULL,
							hours_fare REAL NOT NULL,
							km_fare REAL NOT NULL
						);
					";
				createTableCmd.ExecuteNonQuery();
				createTableCmd.CommandText = @"
						CREATE TABLE IF NOT EXISTS Clients (
							id INTEGER PRIMARY KEY,
							name TEXT NOT NULL,
							surname TEXT NOT NULL,
							email TEXT NOT NULL
						);
					";
				createTableCmd.ExecuteNonQuery();
				createTableCmd.CommandText = @"
						CREATE TABLE IF NOT EXISTS Rentals (
							id INTEGER PRIMARY KEY AUTOINCREMENT,
							start_time DATETIME NOT NULL,
							end_time DATETIME NOT NULL,
							status INTEGER NOT NULL,
							km_driven INTEGER,
							sum REAL,
							car_id INTEGER NOT NULL,
							client_id INTEGER NOT NULL,
							FOREIGN KEY (car_id) REFERENCES Cars (id),
							FOREIGN KEY (client_id) REFERENCES Clients (id)
						);
					";
				createTableCmd.ExecuteNonQuery();
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"An error occurred while creating tables: {ex.Message}");
		}
	}

	//app methods
	public void NewCar(int carId, string model, float hoursFare, float kmsFare)
	{
		try
		{
			using (var connection = new SqliteConnection(connectionString))
			{
				connection.Open();
				// Insert a new rental record into the Rentals table
				using (var command = connection.CreateCommand())
				{
					command.CommandText = @"
                    INSERT INTO Cars (id, model, hours_fare, km_fare)
                    VALUES (@carId, @model, @hoursFare, @kmsFare);
                ";
					// Set parameters for the query
					command.Parameters.AddWithValue("@carId", carId); // Current date-time
					command.Parameters.AddWithValue("@model", model); // Expected end time provided by the user
					command.Parameters.AddWithValue("@hoursFare", hoursFare); // 1=currently active; 0=ended
					command.Parameters.AddWithValue("@kmsFare", kmsFare);
					// Execute the query
					command.ExecuteNonQuery();
				}
				Console.WriteLine($"Car successfully registered.");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"An error occurred while creating the car: {ex.Message}");
		}
	}
	
	public void NewClient(int clientId, string name, string surname, string email)
	{
		try
		{
			using (var connection = new SqliteConnection(connectionString))
			{
				connection.Open();
				// Insert a new rental record into the Rentals table
				using (var command = connection.CreateCommand())
				{
					command.CommandText = @"
                    INSERT INTO Clients (id, name, surname, email)
                    VALUES (@clientId, @name, @surname, @email);
                ";
					// Set parameters for the query
					command.Parameters.AddWithValue("@clientId", clientId); // Current date-time
					command.Parameters.AddWithValue("@name", name); // Expected end time provided by the user
					command.Parameters.AddWithValue("@surname", surname); // 1=currently active; 0=ended
					command.Parameters.AddWithValue("@email", email);
					// Execute the query
					command.ExecuteNonQuery();
				}
				Console.WriteLine($"Client successfully registered.");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"An error occurred while creating the car: {ex.Message}");
		}
	}
	
	public void Rent(int clientId, int carId, string till)
	{
		try
		{
			using (var connection = new SqliteConnection(connectionString))
			{
				connection.Open();
				// Insert a new rental record into the Rentals table
				using (var command = connection.CreateCommand())
				{
					command.CommandText = @"
                    INSERT INTO Rentals (start_time, end_time, status, car_id, client_id)
                    VALUES (@startTime, @endTime, @status, @carId, @clientId);
                ";
					// Set parameters for the query
					command.Parameters.AddWithValue("@startTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")); // Current date-time
					command.Parameters.AddWithValue("@endTime", till); // Expected end time provided by the user
					command.Parameters.AddWithValue("@status", 1); // 1=currently active; 0=ended
					command.Parameters.AddWithValue("@carId", carId);
					command.Parameters.AddWithValue("@clientId", clientId);
					// Execute the query
					command.ExecuteNonQuery();
				}

				rentalCount++;
				Console.WriteLine($"Rental successfully created. Please use the id {rentalCount} for referencing this rental.");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"An error occurred while creating the rental: {ex.Message}");
		}
	}

	public void EndRental(int rentId, int kmDriven)
	{
		try
		{
			using (var connection = new SqliteConnection(connectionString))
			{
				connection.Open();
				// Check if the rental exists and is currently active
				using (var selectCommand = connection.CreateCommand())
				{
					selectCommand.CommandText = @"
                    SELECT id, status 
                    FROM Rentals 
                    WHERE id = @rentalId;
                ";
					selectCommand.Parameters.AddWithValue("@rentalId", rentId);
					using (var reader = selectCommand.ExecuteReader())
					{
						if (!reader.Read())
						{
							Console.WriteLine($"Rental with ID {rentId} does not exist.");
							return;
						}

						int status = reader.GetInt32(1); // Get the status column
						if (status == 0)
						{
							Console.WriteLine($"Rental with ID {rentId} has already ended.");
							return;
						}
					}
				}

				// Update the rental record to mark it as ended
				using (var updateCommand = connection.CreateCommand())
				{
					updateCommand.CommandText = @"
                    UPDATE Rentals
                    SET end_time = @endTime, km_driven = @kmDriven, status = 0
                    WHERE id = @rentalId;
                ";
					updateCommand.Parameters.AddWithValue("@endTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					updateCommand.Parameters.AddWithValue("@kmDriven", kmDriven);
					updateCommand.Parameters.AddWithValue("@rentalId", rentId);
					
					updateCommand.ExecuteNonQuery();
				}
				Console.WriteLine($"Rental with ID {rentId} has been successfully ended...needs to calc sum(not finished)...");
				//calculate sum and add it to Rentals DB
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"An error occurred while ending the rental: {ex.Message}");
		}
	}
}
