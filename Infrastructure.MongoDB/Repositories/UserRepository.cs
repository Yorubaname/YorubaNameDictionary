using Core.Dto.Request;
using Core.Dto.Response;
using Core.Entities;
using System.Linq;
using Core.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

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

        public async Task<bool> DeleteBy(string email)
        {
            var filter = Builders<User>.Filter.Eq(nameof(User.Email), email);
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

        public async Task<bool> Update(string email, UpdateUserDto update)
        {
            var filter = Builders<User>.Filter.Eq(ne => ne.Email, email);
            var updateStatement = GenerateUpdateStatement(update);

            var options = SetCollationPrimary<FindOneAndUpdateOptions<User>> (new FindOneAndUpdateOptions<User>
            {
                ReturnDocument = ReturnDocument.After
            });

            var updated = await _userCollection.FindOneAndUpdateAsync(filter, updateStatement, options);

            return updated != null;
        }


        private static UpdateDefinition<User> GenerateUpdateStatement(UpdateUserDto update)
        {
            // TODO Hafiz later: Is there a more sophisticated and maintainable way to do this without over-engineering?
            var updates = new List<UpdateDefinition<User>>();
            var updateBuilder = Builders<User>.Update;

            if (!string.IsNullOrWhiteSpace(update.Username))
            {
                updates.Add(updateBuilder.Set(u => u.Username, update.Username));
            }

            if (!string.IsNullOrWhiteSpace(update.Password))
            {
                updates.Add(updateBuilder.Set(u => u.Password, update.Password));
            }

            if (update.Roles != null && update.Roles.Any())
            {
                updates.Add(updateBuilder.Set(u => u.Roles, update.Roles.ToList()));
            }

            if (!string.IsNullOrWhiteSpace(update.UpdatedBy))
            {
                updates.Add(updateBuilder.Set(u => u.UpdatedBy, update.UpdatedBy));
            }

            // Always update the UpdatedAt field
            updates.Add(updateBuilder.CurrentDate(u => u.UpdatedAt));

            // Combine all updates into a single update definition
            var updateDefinition = updateBuilder.Combine(updates);

            return updateDefinition;
        }

        public async Task<IEnumerable<UserDto>> List()
        {
            var allUsers = await _userCollection.Find(_ => true).ToListAsync();
            return allUsers.Select(u => new UserDto
            {
                Username = u.Username,
                Email = u.Email!,
                Roles = u.Roles.ToArray()
            });
        }
    }
}
