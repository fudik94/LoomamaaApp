using System;
using System.Collections.Generic;
using System.Linq;
using LoomamaaApp.Klassid;
using LoomamaaApp.Repositories;

namespace LoomamaaApp.Data.Repositories
{
    /// <summary>
    /// In-memory repository for animals using EF Core tracking
    /// </summary>
    public class EfAnimalRepository : IRepository<Animal>
    {
        private readonly LoomamaaDbContext _context;

        public EfAnimalRepository(LoomamaaDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Add(Animal item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _context.Animals.Add(item);
        }

        public void Remove(Animal item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _context.Animals.Remove(item);
        }

        public IEnumerable<Animal> GetAll()
        {
            return _context.Animals.ToList();
        }

        public Animal Find(Func<Animal, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return _context.Animals.FirstOrDefault(predicate);
        }
    }
}
