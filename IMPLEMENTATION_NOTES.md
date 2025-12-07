# LoomamaaApp - Implementation Documentation

## Overview
This application has been enhanced with:
1. Two logging implementations (XML and JSON) via dependency injection
2. Database persistence for Animals and Enclosures using ADO.NET and LocalDB

## Features

### 1. Logging System

The application includes a flexible logging system with two implementations:

#### ILogger Interface
Located in `Logging/ILogger.cs`, this interface defines the contract for all logging implementations:
- `void Log(string message)` - Logs a message
- `void SaveLogs()` - Persists logged messages to storage

#### XmlLogger
Located in `Logging/XmlLogger.cs`
- Logs messages in XML format
- Default file: `application_logs.xml`
- Format:
```xml
<Logs>
  <LogEntry>
    <Timestamp>2024-01-01T12:00:00</Timestamp>
    <Message>Your log message</Message>
  </LogEntry>
</Logs>
```

#### JsonLogger
Located in `Logging/JsonLogger.cs`
- Logs messages in JSON format
- Default file: `application_logs.json`
- Format:
```json
[
  {
    "timestamp": "2024-01-01T12:00:00",
    "message": "Your log message"
  }
]
```

#### Switching Between Loggers

To switch between XML and JSON logging, modify `App.xaml.cs` in the `ConfigureServices()` method:

**For XML logging (default):**
```csharp
container.RegisterSingleton<ILogger>(new XmlLogger("application_logs.xml"));
```

**For JSON logging:**
```csharp
container.RegisterSingleton<ILogger>(new JsonLogger("application_logs.json"));
```

Logs are automatically saved when the application exits.

### 2. Database Persistence

The application now supports saving and loading Animals and Enclosures to/from a LocalDB database.

#### IAnimalDatabaseRepository Interface
Located in `Database/IAnimalDatabaseRepository.cs`, this interface defines database operations:
- `void SaveAnimal(Animal animal)` - Save a single animal
- `void SaveEnclosure(Enclosure<Animal> enclosure)` - Save an enclosure with all its animals
- `IEnumerable<Animal> LoadAnimals()` - Load all animals
- `IEnumerable<Enclosure<Animal>> LoadEnclosures()` - Load all enclosures with their animals
- `void InitializeDatabase()` - Create database tables if they don't exist

#### AnimalDatabaseRepository
Located in `Database/AnimalDatabaseRepository.cs`
- Uses ADO.NET with SqlConnection and SqlCommand
- Connects to LocalDB using the connection string in App.config
- Database file location: `%LOCALAPPDATA%\LoomamaaApp\LoomamaaDB.mdf`

#### Database Schema

**Animals Table:**
- `Id` (INT, Primary Key, Identity)
- `Name` (NVARCHAR(100))
- `Age` (INT)
- `Type` (NVARCHAR(50))
- `EnclosureId` (INT, Nullable)
- `CreatedDate` (DATETIME)

**Enclosures Table:**
- `Id` (INT, Primary Key, Identity)
- `Name` (NVARCHAR(100))
- `CreatedDate` (DATETIME)

#### Using Database Features

1. **Save to Database**: Click the "Save to DB" button to save all enclosures and their animals
   - Note: This will clear all existing data in the database before saving to prevent duplicates
2. **Load from Database**: Click the "Load from DB" button to load previously saved data
   - Note: Loading from database will replace the current in-memory data
   - Event handlers are properly unsubscribed to prevent memory leaks

### 3. Dependency Injection

The application uses a custom `ServiceLocator` class for dependency injection:

**Registered Services:**
- `ILogger` - Logging implementation (XmlLogger or JsonLogger)
- `IRepository<Animal>` - In-memory animal repository
- `IAnimalDatabaseRepository` - Database repository for persistence

Services are configured in `App.xaml.cs` in the `OnStartup` method and resolved in `MainWindow.xaml.cs`.

**Thread Safety:**
The ServiceLocator uses double-check locking to ensure thread-safe singleton initialization.

## Quality Assurance

### Code Review
All code has been reviewed and the following improvements were made:
- **Memory Leak Prevention**: Added UnsubscribeEnclosure method to properly clean up event handlers when loading from database
- **Thread Safety**: Implemented double-check locking in ServiceLocator for thread-safe singleton initialization
- **Data Integrity**: Database is cleared before saving to prevent duplicate records
- **Code Clarity**: Updated comments to match actual implementation

### Security Analysis
CodeQL security analysis completed with **0 vulnerabilities** found.
- No SQL injection vulnerabilities (all queries use parameterized commands)
- No path traversal issues
- No insecure data handling
- Proper exception handling throughout

## Configuration

### Database Connection String
Located in `App.config`:
```xml
<connectionStrings>
  <add name="LoomamaaDB" 
       connectionString="Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\LoomamaaDB.mdf;Integrated Security=True;Connect Timeout=30" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

The `|DataDirectory|` placeholder is set to `%LOCALAPPDATA%\LoomamaaApp` at runtime.

## Requirements

- .NET Framework 4.7.2
- SQL Server LocalDB (included with Visual Studio)
- Windows OS

## Architecture Changes

### Before
- MainViewModel directly instantiated dependencies
- No logging system
- No database persistence

### After
- Dependencies injected via ServiceLocator
- ILogger interface with XML/JSON implementations
- IAnimalDatabaseRepository interface with ADO.NET implementation
- All logging and database operations integrated with the existing UI

## Event Logging

The following events are now logged:
- Application start
- Animal added to enclosure
- Food dropped in enclosure
- Night events
- Animal sound made
- Animal fed
- New animal created
- Animal removed
- Crazy action performed
- Database save operations
- Database load operations

All logs can be viewed in the Logs panel at the bottom of the application window.
