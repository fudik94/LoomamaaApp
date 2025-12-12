using System;
using System.Collections.Generic;
using System.Linq;
using LoomamaaApp.Klassid;

namespace LoomamaaApp.Repositories
{
    public class AnimalRepository : IRepository<Animal>
    {
        private readonly List<Animal> animals = new List<Animal>();

        public void Add(Animal item) => animals.Add(item);

        public void Remove(Animal item) => animals.Remove(item);

        public IEnumerable<Animal> GetAll() => animals;

        public Animal Find(Func<Animal, bool> predicate) => animals.FirstOrDefault(predicate);
    }
}
