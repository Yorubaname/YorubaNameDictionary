using Xunit;

namespace Test;

[CollectionDefinition("Shared_Test_Collection")]
public class SharedTestCollection : ICollectionFixture<BootStrappedApiFactory>
{
}