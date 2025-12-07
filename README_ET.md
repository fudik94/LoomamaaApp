# LoomamaaApp - Ülesande Lahendus

## Nõuded ja Lahendused

### 1. Kahe realisatsiooniga logimissüsteem

**✅ Nõue täidetud:**
- Loodud ühtne `ILogger` interfacе (Logging/ILogger.cs)
- Kaks realisatsiooni:
  - `XmlLogger` - salvestab logid XML kujul
  - `JsonLogger` - salvestab logid JSON kujul
- Mõlemad pärinevad samast interface-ist (ILogger)
- Kasutatav realisatsioon määratakse DI konteineri abil

**Kuidas vahetada XML ja JSON vahel:**

Muuda failis `App.xaml.cs` meetodit `ConfigureServices()`:

```csharp
// XML logimiseks:
container.RegisterSingleton<ILogger>(new XmlLogger("application_logs.xml"));

// JSON logimiseks:
container.RegisterSingleton<ILogger>(new JsonLogger("application_logs.json"));
```

### 2. LocalDB andmebaas ADO.NET põhimõttel

**✅ Nõue täidetud:**
- Salvestab loomade ja voljeeride info LocalDB andmebaasi
- Kasutab ADO.NET põhimõtteid (SqlConnection, SqlCommand)
- Loodud `IAnimalDatabaseRepository` interface
- Realisatsioon `AnimalDatabaseRepository` pärineb interface-ist
- Kasutatav DI abil (ServiceLocator konteineri kaudu)

**Andmebaasi funktsioonid:**
- "Save to DB" nupp - salvestab kõik voljeerid ja loomad andmebaasi
- "Load from DB" nupp - laeb andmebaasi salvestatud andmed
- Automaatne andmebaasi skeemi loomine
- Kaitse duplikaatandmete vastu

## Projekti struktuur

### Uued kaustad ja failid:

```
LoomamaaApp/
├── Logging/
│   ├── ILogger.cs              # Logimise interface
│   ├── XmlLogger.cs            # XML logimise realisatsioon
│   └── JsonLogger.cs           # JSON logimise realisatsioon
├── Database/
│   ├── IAnimalDatabaseRepository.cs    # Andmebaasi interface
│   └── AnimalDatabaseRepository.cs     # ADO.NET realisatsioon
├── ServiceLocator.cs           # DI konteiner
└── IMPLEMENTATION_NOTES.md     # Detailne dokumentatsioon
```

### Muudetud failid:

- `App.config` - Lisatud LocalDB ühendusstring
- `App.xaml.cs` - Lisatud DI konfiguratsioon
- `MainWindow.xaml.cs` - Kasutab DI-d sõltuvuste saamiseks
- `MainWindow.xaml` - Lisatud Save/Load nupud
- `ViewModels/MainViewModel.cs` - Kasutab constructor injection-it

## DI (Dependency Injection) kasutamine

Kõik sõltuvused registreeritakse `App.xaml.cs` failis:

```csharp
private void ConfigureServices()
{
    var container = ServiceLocator.Instance;

    // Logger registreerimine (vali XML või JSON)
    container.RegisterSingleton<ILogger>(new XmlLogger("application_logs.xml"));
    
    // In-memory repository
    container.RegisterSingleton<IRepository<Animal>>(new AnimalRepository());

    // Andmebaasi repository
    string connectionString = ConfigurationManager.ConnectionStrings["LoomamaaDB"].ConnectionString;
    var dbRepo = new AnimalDatabaseRepository(connectionString);
    dbRepo.InitializeDatabase();
    container.RegisterSingleton<IAnimalDatabaseRepository>(dbRepo);
}
```

Sõltuvused süstitakse `MainViewModel`-isse konstruktori kaudu:

```csharp
public MainViewModel(ILogger logger, IRepository<Animal> repository, IAnimalDatabaseRepository dbRepository)
{
    this.logger = logger;
    this.repository = repository;
    this.dbRepository = dbRepository;
    // ...
}
```

## Andmebaasi skeem

**Animals tabel:**
- Id (INT, Primary Key)
- Name (NVARCHAR)
- Age (INT)
- Type (NVARCHAR)
- EnclosureId (INT)
- CreatedDate (DATETIME)

**Enclosures tabel:**
- Id (INT, Primary Key)
- Name (NVARCHAR)
- CreatedDate (DATETIME)

## Logimise funktsioonid

Rakendus logib järgmisi sündmusi:
- Rakenduse käivitamine
- Looma lisamine voljeeri
- Toidu andmine
- Looma hääl
- Öösündmused
- Andmebaasi operatsioonid
- Loomade lisamine/eemaldamine

Logid salvestatakse automaatselt rakenduse sulgemisel.

## Kvaliteedikontroll

- ✅ Koodi ülevaatus tehtud ja kõik probleemid parandatud
- ✅ CodeQL turvakontroll: 0 haavatavust
- ✅ Mälulekete vältimine
- ✅ Thread-safe singleton implementatsioon
- ✅ SQL injection kaitse (kõik päringud kasutavad parameetreid)

## Tehnilised detailid

- .NET Framework 4.7.2
- WPF (Windows Presentation Foundation)
- ADO.NET
- SQL Server LocalDB
- MVVM pattern
- Dependency Injection

## Kasutamine

1. **Logimise vahetamine**: Muuda `App.xaml.cs` failis logger registreerimist
2. **Andmete salvestamine**: Vajuta "Save to DB" nuppu
3. **Andmete laadimine**: Vajuta "Load from DB" nuppu
4. **Logide vaatamine**: Logid kuvatakse rakenduse alumises paneellis

Täielik dokumentatsioon inglise keeles: `IMPLEMENTATION_NOTES.md`

## Kokkuvõte

Mõlemad nõuded on edukalt täidetud:

1. ✅ **Kaks logimise realisatsiooni** (XML ja JSON) ühe interface-i põhjal, kasutades DI-d
2. ✅ **Andmebaasi salvestamine** ADO.NET abil LocalDB-sse, repository interface-i põhjal, kasutades DI-d

Implementatsioon järgib SOLID põhimõtteid ja on tootmiskõlblik (0 turvahaavatavust).
