using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.NameEntry;
using Core.Entities.NameEntry.Collections;
using Core.Enums;
using Infrastructure.MongoDB.Repositories;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Xunit;
using Xunit.Abstractions;

namespace Test.RepositoryTest;
// [CollectionDefinition("Mongo Database")]
// public class MongoDatabaseDefinition : ICollectionFixture<IMongoDatabase>
// {
//     
// }


public class NameEntryRepositoryTest :  IClassFixture<IMongoDatabaseHelper>, IDisposable
{
    private readonly NameEntryRepository _sut;
    private readonly ITestOutputHelper _outputHelper;
    
    public NameEntryRepositoryTest(ITestOutputHelper outputHelper, IMongoDatabaseHelper mongoDatabaseHelper)
    {
        _outputHelper = outputHelper;
        _sut = new NameEntryRepository(mongoDatabaseHelper.MongoDatabase());
    }

    [Theory]
    [MemberData(nameof(CreateNameEntryTestData))]
    public async Task Test1(NameEntry nameEntry)
    {
        await _sut.Create(nameEntry);
        var savedNameEntry = await _sut.FindById(nameEntry.Id);
        
        Assert.NotNull(savedNameEntry);
        Assert.Equal(nameEntry.Name, savedNameEntry.Name);
    }

    private static IEnumerable<object[]> CreateNameEntryTestData()
    {
        return new List<object[]>
        {
            new object[]
            {
                new NameEntry
                {
                    Name = "SampleName",
                    Pronunciation = "SamplePronunciation",
                    IpaNotation = "SampleIpaNotation",
                    Meaning = "SampleMeaning",
                    ExtendedMeaning = "SampleExtendedMeaning",
                    Morphology = new List<string> { "Morphology1", "Morphology2" },
                    Media = new List<string> { "Media1.jpg", "Media2.jpg" },
                    State = State.NEW,
                    Etymology = new List<Etymology> { new Etymology("Nigerian", "Local") },
                    Videos = new List<EmbeddedVideo>
                        { new EmbeddedVideo("ID", "Caption"), new EmbeddedVideo("ID2", "Caption2") },
                    GeoLocation = new List<GeoLocation> { new GeoLocation(), new GeoLocation() },
                    FamousPeople = new List<string> { "Person1", "Person2" },
                    Syllables = new List<string> { "Syllable1", "Syllable2" },
                    Variants = new List<string> { "Variant1", "Variant2" },
                    Modified = default,
                    Duplicates = new List<NameEntry> { new NameEntry(), new NameEntry() },
                    Feedbacks = new List<Feedback> { new Feedback(), new Feedback() }
                }
            }
        };
    }


    public void Dispose()
    {
        
    }
}