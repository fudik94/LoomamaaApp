using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using LoomamaaApp.Logging;
using LoomamaaApp.Database;
using LoomamaaApp.Repositories;
using LoomamaaApp.Klassid;
using LoomamaaApp.Data;
using LoomamaaApp.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LoomamaaApp
{
    /// logic kind of
    
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set DataDirectory for LocalDB
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "LoomamaaApp");
            
            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);
                
            AppDomain.CurrentDomain.SetData("DataDirectory", appDataPath);

            // Configure DI Container
            ConfigureServices();
        }

        private void ConfigureServices()
        {
            var container = ServiceLocator.Instance;

            // Register logger - you can switch between XmlLogger and JsonLogger here
            // To use XML logging:
            container.RegisterSingleton<ILogger>(new XmlLogger("application_logs.xml"));
            
            // To use JSON logging instead, comment the line above and uncomment this:
            // container.RegisterSingleton<ILogger>(new JsonLogger("application_logs.json"));

            // Configure Entity Framework Core DbContext
            string connectionString = ConfigurationManager.ConnectionStrings["LoomamaaDB"].ConnectionString;
            
            var optionsBuilder = new DbContextOptionsBuilder<LoomamaaDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            
            var dbContext = new LoomamaaDbContext(optionsBuilder.Options);
            
            // Register DbContext as singleton
            container.RegisterSingleton<LoomamaaDbContext>(dbContext);

            // Register EF Core repositories
            container.RegisterSingleton<IRepository<Animal>>(new EfAnimalRepository(dbContext));
            
            var dbRepo = new EfAnimalDatabaseRepository(dbContext);
            
            // Initialize database on startup
            try
            {
                dbRepo.InitializeDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Database initialization failed: {ex.Message}\n\nThe application will continue with in-memory storage only.",
                    "Database Warning",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            
            container.RegisterSingleton<IAnimalDatabaseRepository>(dbRepo);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Save logs on exit
            try
            {
                var logger = ServiceLocator.Instance.Resolve<ILogger>();
                logger.SaveLogs();
            }
            catch
            {
                // Ignore errors during shutdown
            }

            base.OnExit(e);
        }
    }
}
