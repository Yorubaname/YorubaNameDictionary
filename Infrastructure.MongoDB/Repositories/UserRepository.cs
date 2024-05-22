using Core.Entities;
using Core.Entities.NameEntry;
using Core.Enums;
using Core.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Xml.Linq;

namespace Infrastructure.MongoDB.Repositories
{
    public class UserRepository : MongoDBRepository, IUserRepository
    {
        private readonly IMongoCollection<User> _userCollection;

        public UserRepository(
            IMongoDatabase database)
        {
            _userCollection = database.GetCollection<User>("Users");
        }

        public async Task Create(User theUser)
        {
            theUser.Id = ObjectId.GenerateNewId().ToString();
            await _userCollection.InsertOneAsync(theUser);
        }

        public async Task<bool> DeleteBy(string username)
        {
            var filter = Builders<User>.Filter.Eq(nameof(User.Email), username);
            var deleteResult = await _userCollection.DeleteOneAsync(filter, SetCollationPrimary<DeleteOptions>(new DeleteOptions()));
            return deleteResult.DeletedCount > 0;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var filter = Builders<User>.Filter.Eq(nameof(User.Email), email);
            var options = new FindOptions
            {
                Collation = new Collation("en", strength: CollationStrength.Primary)
            };

            return await _userCollection.Find(filter, options).SingleOrDefaultAsync();
        }
    }
}
