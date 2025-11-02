using System;
using System.Collections.Generic;

namespace LoomamaaApp.Repositories
{
    public interface IRepository<T>
    {
        void Add(T item);
        void Remove(T item);
        IEnumerable<T> GetAll();
        // Returns default(T) (usually null for reference types) when not found
        T Find(Func<T, bool> predicate);
    }
}
