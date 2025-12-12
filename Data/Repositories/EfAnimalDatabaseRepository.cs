using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LoomamaaApp.Klassid;
using LoomamaaApp.Database;

namespace LoomamaaApp.Data.Repositories
{
    /// <summary>
    /// Entity Framework Core implementation of animal database repository
    /// </summary>
    public class EfAnimalDatabaseRepository : IAnimalDatabaseRepository
    {
        private readonly LoomamaaDbContext _context;

        public EfAnimalDatabaseRepository(LoomamaaDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void InitializeDatabase()
        {
            try
            {
                // Ensure database is created
                _context.Database.EnsureCreated();
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
                if (animal == null) throw new ArgumentNullException(nameof(animal));

                // Add animal without enclosure
                animal.EnclosureId = null;
                _context.Animals.Add(animal);
                _context.SaveChanges();
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
                if (enclosure == null) throw new ArgumentNullException(nameof(enclosure));

                // Create enclosure entity
                var enclosureEntity = new EnclosureEntity
                {
                    Name = enclosure.Name ?? "Unknown",
                    CreatedDate = DateTime.Now
                };

                _context.Enclosures.Add(enclosureEntity);
                _context.SaveChanges();

                // Save all animals in the enclosure
                foreach (var animal in enclosure.Animals)
                {
                    animal.EnclosureId = enclosureEntity.Id;
                    _context.Animals.Add(animal);
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving enclosure: {ex.Message}");
                throw;
            }
        }

        public IEnumerable<Animal> LoadAnimals()
        {
            try
            {
                // Load animals without enclosure
                return _context.Animals
                    .Where(a => a.EnclosureId == null)
                    .ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading animals: {ex.Message}");
                return new List<Animal>();
            }
        }

        public IEnumerable<Enclosure<Animal>> LoadEnclosures()
        {
            try
            {
                var enclosures = new List<Enclosure<Animal>>();

                // Load all enclosures
                var enclosureEntities = _context.Enclosures.ToList();

                foreach (var enclosureEntity in enclosureEntities)
                {
                    var enclosure = new Enclosure<Animal>(enclosureEntity.Name);

                    // Load animals for this enclosure
                    var animals = _context.Animals
                        .Where(a => a.EnclosureId == enclosureEntity.Id)
                        .ToList();

                    foreach (var animal in animals)
                    {
                        enclosure.Animals.Add(animal);
                    }

                    enclosures.Add(enclosure);
                }

                return enclosures;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading enclosures: {ex.Message}");
                return new List<Enclosure<Animal>>();
            }
        }

        public void DeleteAnimal(int id)
        {
            try
            {
                var animal = _context.Animals.Find(id);
                if (animal != null)
                {
                    _context.Animals.Remove(animal);
                    _context.SaveChanges();
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
                // Delete animals in the enclosure
                var animals = _context.Animals.Where(a => a.EnclosureId == id).ToList();
                _context.Animals.RemoveRange(animals);

                // Delete enclosure
                var enclosure = _context.Enclosures.Find(id);
                if (enclosure != null)
                {
                    _context.Enclosures.Remove(enclosure);
                }

                _context.SaveChanges();
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
                // Delete all animals
                _context.Animals.RemoveRange(_context.Animals);

                // Delete all enclosures
                _context.Enclosures.RemoveRange(_context.Enclosures);

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing database: {ex.Message}");
                throw;
            }
        }
    }
}
