using Application.Migrator;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MigratorController : Controller
    {
        private readonly SQLToMongoMigrator _sQLToMongoMigrator;
        public MigratorController(SQLToMongoMigrator sQLToMongoMigrator)
        {
            _sQLToMongoMigrator = sQLToMongoMigrator;
        }
        [HttpGet("MigrateGeoLocation")]
        public IActionResult MigrateGeoLocation()
        {
           StringBuilder results = new StringBuilder();
           results.AppendLine(_sQLToMongoMigrator.MigrateGeolocation());
           return Ok(results.ToString());
          
        }
    }
}