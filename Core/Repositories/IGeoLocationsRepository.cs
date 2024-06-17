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
    }
}
