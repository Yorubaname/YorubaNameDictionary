using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Cache
{
    public interface ICache<T>
    {
        Task<IEnumerable<T>> Get();
        Task Stack(T item);
        Task<bool> Remove(T item);
    }
}
