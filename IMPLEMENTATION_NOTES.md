# LoomamaaApp - Implementation Documentation

## Overview
This application has been enhanced with:
1. Two logging implementations (XML and JSON) via dependency injection
2. Database persistence for Animals and Enclosures using Entity Framework Core and LocalDB
3. Clean architecture following DDD and SOLID principles

## Migration from ADO.NET to Entity Framework Core

### What Changed
The application has been migrated from ADO.NET to Entity Framework Core while maintaining the same functionality and interfaces. This migration provides:

**✓ Removed**: Old ADO.NET implementation
- Deleted `Database/AnimalDatabaseRepository.cs` (used SqlConnection, SqlCommand)
- Eliminated all raw SQL query strings
- Removed manual parameter binding and data reader code

**✓ Added**: Entity Framework Core infrastructure
- `Data/LoomamaaDbContext.cs` - EF Core database context
- `Data/Repositories/EfAnimalRepository.cs` - EF-based in-memory repository
- `Data/Repositories/EfAnimalDatabaseRepository.cs` - EF-based database repository
- Entity Framework Core 3.1.32 NuGet packages

**✓ Preserved**: All interfaces and contracts
- `IRepository<Animal>` - unchanged interface
- `IAnimalDatabaseRepository` - unchanged interface
- All ViewModels work without modifications (Dependency Inversion Principle)

**✓ Enhanced**: Architecture and design
- **Clean Architecture**: Clear separation between Domain, Infrastructure, and UI
- **DDD**: Domain entities (Animal) are independent of database concerns
- **SOLID Principles**: SRP, OCP, and DIP applied throughout
- **Type Safety**: LINQ queries provide compile-time checking
- **Security**: No SQL injection risks - EF Core handles parameterization
- **Maintainability**: Less boilerplate code, easier to extend

### Benefits
1. **Cleaner Code**: No manual SQL strings or data readers
2. **Type Safety**: LINQ queries checked at compile time
3. **Better Testing**: DbContext can be mocked easily
4. **Security**: Automatic parameterization prevents SQL injection
5. **Flexibility**: Easy to swap database providers
6. **Maintainability**: Less code to maintain, clearer intent

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

### 2. Database Persistence with Entity Framework Core

The application uses Entity Framework Core 3.1 for database persistence, supporting Animals and Enclosures in LocalDB.

#### LoomamaaDbContext
Located in `Data/LoomamaaDbContext.cs`
- EF Core DbContext managing database operations
- Implements Table Per Hierarchy (TPH) inheritance for Animal types
- Configures entity mappings using Fluent API
- Supports Cat, Dog, Bear, Duck, Horse, Monkey, Pig, and Sheep entities

#### IAnimalDatabaseRepository Interface
Located in `Database/IAnimalDatabaseRepository.cs`, this interface defines database operations:
- `void SaveAnimal(Animal animal)` - Save a single animal
- `void SaveEnclosure(Enclosure<Animal> enclosure)` - Save an enclosure with all its animals
- `IEnumerable<Animal> LoadAnimals()` - Load all animals
- `IEnumerable<Enclosure<Animal>> LoadEnclosures()` - Load all enclosures with their animals
- `void InitializeDatabase()` - Create database tables if they don't exist

#### EfAnimalDatabaseRepository
Located in `Data/Repositories/EfAnimalDatabaseRepository.cs`
- Entity Framework Core implementation of IAnimalDatabaseRepository
- Uses DbContext for all database operations
- No raw SQL queries - all operations use LINQ and EF Core APIs
- Connects to LocalDB using the connection string in App.config
- Database file location: `%LOCALAPPDATA%\LoomamaaApp\LoomamaaDB.mdf`

#### EfAnimalRepository
Located in `Data/Repositories/EfAnimalRepository.cs`
- In-memory repository implementation using EF Core change tracking
- Implements IRepository<Animal> interface
- Provides Add, Remove, GetAll, and Find operations

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
- `LoomamaaDbContext` - Entity Framework Core database context
- `ILogger` - Logging implementation (XmlLogger or JsonLogger)
- `IRepository<Animal>` - EF Core-based animal repository
- `IAnimalDatabaseRepository` - EF Core database repository for persistence

Services are configured in `App.xaml.cs` in the `OnStartup` method using the following pattern:
1. DbContext is created with SQL Server options
2. Repositories receive the DbContext via constructor injection
3. All services are registered as singletons in ServiceLocator

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
- No SQL injection vulnerabilities (EF Core uses parameterized queries automatically)
- No path traversal issues
- No insecure data handling
- Proper exception handling throughout
- Migration from ADO.NET to EF Core eliminates manual SQL query construction

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
- Entity Framework Core 3.1.32 (compatible with .NET Framework)
- SQL Server LocalDB (included with Visual Studio)
- Windows OS

## Architecture Changes

### Before (Initial Version)
- MainViewModel directly instantiated dependencies
- No logging system
- No database persistence

### After (First Enhancement - ADO.NET)
- Dependencies injected via ServiceLocator
- ILogger interface with XML/JSON implementations
- IAnimalDatabaseRepository interface with ADO.NET implementation
- All logging and database operations integrated with the existing UI

### Current (Entity Framework Core Migration)
- **Clean Architecture**: Separation of concerns with Data, Domain, and UI layers
- **DDD Principles**: Domain entities are independent of database concerns
- **SOLID Principles**:
  - **SRP**: Each class has single responsibility (DbContext, repositories, entities)
  - **OCP**: Architecture is open for extension (new repositories) without modifying existing code
  - **DIP**: UI and services depend on interfaces (IRepository, IAnimalDatabaseRepository)
- **EF Core Infrastructure**:
  - LoomamaaDbContext manages database operations
  - EfAnimalRepository implements in-memory operations with EF tracking
  - EfAnimalDatabaseRepository implements database persistence
  - Table Per Hierarchy (TPH) inheritance for Animal types
- **No raw SQL**: All database operations use LINQ and EF Core APIs
- **Type Safety**: Compile-time checking for database queries
- **Better Testability**: Repositories can be mocked for unit testing

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
