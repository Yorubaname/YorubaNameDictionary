using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Core.Entities.NameEntry;
using Core.Enums;

namespace Test.Integration.NameController.Data;

public class NamesAllTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] {NameEntries()};
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private static List<NameEntry> NameEntries()
    {
        var fixture = new Fixture();

        fixture.Customize<NameEntry>(c => c
            .With(x => x.State, State.PUBLISHED)
            .With(ne => ne.Modified, (NameEntry?)default)
            .With(ne => ne.Duplicates, [])
        );

        return fixture.CreateMany<NameEntry>(2).ToList();
    }
}