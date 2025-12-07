using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using LoomamaaApp.Klassid;

namespace LoomamaaApp.Database
{
    public class AnimalDatabaseRepository : IAnimalDatabaseRepository
    {
        private readonly string connectionString;

        public AnimalDatabaseRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void InitializeDatabase()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Create Animals table
                    var createAnimalsTable = @"
                        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Animals')
                        BEGIN
                            CREATE TABLE Animals (
                                Id INT IDENTITY(1,1) PRIMARY KEY,
                                Name NVARCHAR(100) NOT NULL,
                                Age INT NOT NULL,
                                Type NVARCHAR(50) NOT NULL,
                                EnclosureId INT NULL,
                                CreatedDate DATETIME DEFAULT GETDATE()
                            )
                        END";

                    using (var cmd = new SqlCommand(createAnimalsTable, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Create Enclosures table
                    var createEnclosuresTable = @"
                        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Enclosures')
                        BEGIN
                            CREATE TABLE Enclosures (
                                Id INT IDENTITY(1,1) PRIMARY KEY,
                                Name NVARCHAR(100) NOT NULL,
                                CreatedDate DATETIME DEFAULT GETDATE()
                            )
                        END";

                    using (var cmd = new SqlCommand(createEnclosuresTable, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing database: {ex.Message}");
                throw;
            }
        }

        public void SaveAnimal(Animal animal)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    var query = @"INSERT INTO Animals (Name, Age, Type) VALUES (@Name, @Age, @Type)";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", animal.Name);
                        cmd.Parameters.AddWithValue("@Age", animal.Age);
                        cmd.Parameters.AddWithValue("@Type", animal.TypeName);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving animal: {ex.Message}");
                throw;
            }
        }

        public void SaveEnclosure(Enclosure<Animal> enclosure)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // First, insert the enclosure
                    var insertEnclosureQuery = @"
                        INSERT INTO Enclosures (Name) VALUES (@Name);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    int enclosureId;
                    using (var cmd = new SqlCommand(insertEnclosureQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", enclosure.Name ?? "Unknown");
                        enclosureId = (int)cmd.ExecuteScalar();
                    }

                    // Then, save all animals in the enclosure
                    foreach (var animal in enclosure.Animals)
                    {
                        var insertAnimalQuery = @"
                            INSERT INTO Animals (Name, Age, Type, EnclosureId) 
                            VALUES (@Name, @Age, @Type, @EnclosureId)";

                        using (var cmd = new SqlCommand(insertAnimalQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@Name", animal.Name);
                            cmd.Parameters.AddWithValue("@Age", animal.Age);
                            cmd.Parameters.AddWithValue("@Type", animal.TypeName);
                            cmd.Parameters.AddWithValue("@EnclosureId", enclosureId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving enclosure: {ex.Message}");
                throw;
            }
        }

        public IEnumerable<Animal> LoadAnimals()
        {
            var animals = new List<Animal>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    var query = @"SELECT Id, Name, Age, Type FROM Animals WHERE EnclosureId IS NULL";

                    using (var cmd = new SqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var name = reader.GetString(reader.GetOrdinal("Name"));
                            var age = reader.GetInt32(reader.GetOrdinal("Age"));
                            var type = reader.GetString(reader.GetOrdinal("Type"));

                            var animal = CreateAnimalByType(type, name, age);
                            if (animal != null)
                                animals.Add(animal);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading animals: {ex.Message}");
            }

            return animals;
        }

        public IEnumerable<Enclosure<Animal>> LoadEnclosures()
        {
            var enclosures = new List<Enclosure<Animal>>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Load all enclosures
                    var enclosureQuery = @"SELECT Id, Name FROM Enclosures";
                    var enclosureDict = new Dictionary<int, Enclosure<Animal>>();

                    using (var cmd = new SqlCommand(enclosureQuery, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32(reader.GetOrdinal("Id"));
                            var name = reader.GetString(reader.GetOrdinal("Name"));
                            var enclosure = new Enclosure<Animal>(name);
                            enclosureDict[id] = enclosure;
                            enclosures.Add(enclosure);
                        }
                    }

                    // Load animals for each enclosure
                    var animalQuery = @"SELECT Id, Name, Age, Type, EnclosureId FROM Animals WHERE EnclosureId IS NOT NULL";

                    using (var cmd = new SqlCommand(animalQuery, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var name = reader.GetString(reader.GetOrdinal("Name"));
                            var age = reader.GetInt32(reader.GetOrdinal("Age"));
                            var type = reader.GetString(reader.GetOrdinal("Type"));
                            var enclosureId = reader.GetInt32(reader.GetOrdinal("EnclosureId"));

                            var animal = CreateAnimalByType(type, name, age);
                            if (animal != null && enclosureDict.ContainsKey(enclosureId))
                            {
                                enclosureDict[enclosureId].Animals.Add(animal);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading enclosures: {ex.Message}");
            }

            return enclosures;
        }

        public void DeleteAnimal(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    var query = @"DELETE FROM Animals WHERE Id = @Id";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting animal: {ex.Message}");
                throw;
            }
        }

        public void DeleteEnclosure(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Delete animals in the enclosure
                    var deleteAnimalsQuery = @"DELETE FROM Animals WHERE EnclosureId = @Id";
                    using (var cmd = new SqlCommand(deleteAnimalsQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                    }

                    // Delete enclosure
                    var deleteEnclosureQuery = @"DELETE FROM Enclosures WHERE Id = @Id";
                    using (var cmd = new SqlCommand(deleteEnclosureQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting enclosure: {ex.Message}");
                throw;
            }
        }

        public void ClearDatabase()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Delete all animals first
                    var deleteAnimalsQuery = @"DELETE FROM Animals";
                    using (var cmd = new SqlCommand(deleteAnimalsQuery, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Delete all enclosures
                    var deleteEnclosuresQuery = @"DELETE FROM Enclosures";
                    using (var cmd = new SqlCommand(deleteEnclosuresQuery, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing database: {ex.Message}");
                throw;
            }
        }

        private Animal CreateAnimalByType(string type, string name, int age)
        {
            switch (type)
            {
                case "Cat":
                    return new Cat(name, age);
                case "Dog":
                    return new Dog(name, age);
                case "Bear":
                    return new Bear(name, age);
                case "Duck":
                    return new Duck(name, age);
                case "Horse":
                    return new Horse(name, age);
                case "Monkey":
                    return new Monkey(name, age);
                case "Pig":
                    return new Pig(name, age);
                case "Sheep":
                    return new Sheep(name, age);
                default:
                    System.Diagnostics.Debug.WriteLine($"Unknown animal type: {type}");
                    return null;
            }
        }
    }
}
