using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Core.Entities.NameEntry;
using Core.Enums;

namespace Test.Integration.NameController.Data;

public class NamesStateTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { NameEntries(), State.NEW };
        yield return new object[] { NameEntries(), State.MODIFIED };
        yield return new object[] { NameEntries(), State.PUBLISHED };
        yield return new object[] { NameEntries(), State.UNPUBLISHED };
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