using Application.Migrator;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MigratorController : Controller
    {
        private readonly SqlToMongoMigrator _sQLToMongoMigrator;
        public MigratorController(SqlToMongoMigrator sQLToMongoMigrator)
        {
            _sQLToMongoMigrator = sQLToMongoMigrator;
        }
        [HttpGet("MigrateGeoLocation")]
        public IActionResult MigrateGeoLocation()
        {
           string result = _sQLToMongoMigrator.MigrateGeolocation();
           return Ok(result);
          
        }

        [HttpGet("MigrateNameEntry")]
        public IActionResult MigrateNameEntry()
        {
            string result = _sQLToMongoMigrator.MigrateNameEntry();
            return Ok(result);

        }
    }
}