namespace Infrastructure.MongoDB
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class MongoCollectionNameAttribute : Attribute
    {
        public string Name { get; }
        public MongoCollectionNameAttribute(string name) => Name = name;
    }
}
