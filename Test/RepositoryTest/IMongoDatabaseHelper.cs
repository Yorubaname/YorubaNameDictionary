using MongoDB.Driver;

namespace Test.RepositoryTest;

public class IMongoDatabaseHelper 
{
    private readonly IMongoDatabase _database;
    public IMongoDatabaseHelper()
    {
        _database = new MongoClient("mongodb://localhost:27018")
            .GetDatabase("testDatabase");
    }

    public IMongoDatabase MongoDatabase()
    {
        return _database;
    }
}