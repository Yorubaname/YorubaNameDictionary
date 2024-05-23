using MongoDB.Driver;

namespace Infrastructure.MongoDB.Repositories
{
    public abstract class MongoDBRepository
    {
        protected static T SetCollationPrimary<T>(dynamic dbCommandOption)
        {
            dbCommandOption.Collation = new Collation("en", strength: CollationStrength.Primary);
            return (T)dbCommandOption;
        }
    }
}