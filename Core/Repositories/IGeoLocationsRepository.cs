using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories
{
    public interface IGeoLocationsRepository
    {
        Task<List<GeoLocation>> GetAll();

        Task<GeoLocation> FindByPlace(string place);
<<<<<<< HEAD
        Task<GeoLocation> FindByRegion(string region);
=======
>>>>>>> b5e889cfd7992c57908bab3da26b61d5aaea9a52
    }
}
