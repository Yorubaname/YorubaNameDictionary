using Application.Migrator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Policy = "AdminOnly")]
    public class MigratorController : Controller
    {
        private readonly SqlToMongoMigrator _sqlToMongoMigrator;

        public MigratorController(SqlToMongoMigrator sqlToMongoMigrator)
        {
            _sqlToMongoMigrator = sqlToMongoMigrator;
        }

        [HttpGet("MigrateGeoLocation")]
        public IActionResult MigrateGeoLocation()
        {
           string result = _sqlToMongoMigrator.MigrateGeolocation();
           return Ok(result);
        }

        [HttpGet("MigrateNameEntry")]
        public IActionResult MigrateNameEntry()
        {
            string result = _sqlToMongoMigrator.MigrateNameEntry();
            return Ok(result);
        }

        [HttpGet("MigrateSuggestedNames")]
        public IActionResult MigrateSuggestedNames()
        {
            string result = _sqlToMongoMigrator.MigrateSuggestedNames();
            return Ok(result);
        }

        [HttpGet("MigrateUsers")]
        public IActionResult MigrateUsers()
        {
            string result = _sqlToMongoMigrator.MigrateUsers();
            return Ok(result);
        }

        
    }
}