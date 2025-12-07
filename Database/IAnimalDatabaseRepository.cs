using System.Collections.Generic;
using LoomamaaApp.Klassid;

namespace LoomamaaApp.Database
{
    public interface IAnimalDatabaseRepository
    {
        void SaveAnimal(Animal animal);
        void SaveEnclosure(Enclosure<Animal> enclosure);
        IEnumerable<Animal> LoadAnimals();
        IEnumerable<Enclosure<Animal>> LoadEnclosures();
        void DeleteAnimal(int id);
        void DeleteEnclosure(int id);
        void ClearDatabase();
        void InitializeDatabase();
    }
}
