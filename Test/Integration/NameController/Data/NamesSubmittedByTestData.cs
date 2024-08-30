using System.Collections;
using System.Collections.Generic;
using AutoFixture;
using Core.Entities.NameEntry;
using Core.Enums;

namespace Test.Integration.NameController.Data;

public class NamesSubmittedByTestData : IEnumerable<object[]>
{
    private const string CreatedByAdeshina = "Adeshina";
    private const string CreatedByIsmaila = "Ismaila";

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] {NameEntries(), CreatedByAdeshina};
        yield return new object[] {NameEntries(), CreatedByIsmaila};
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private static List<NameEntry> NameEntries()
    {
        var fixture = new Fixture();
        return
        [
            fixture.Build<NameEntry>()
                   .With(ne => ne.State, State.PUBLISHED)
                   .With(ne => ne.CreatedBy, CreatedByAdeshina)
                   .With(ne => ne.Modified, (NameEntry?)default)
                   .With(ne => ne.Duplicates, [])
                   .Create(),

            fixture.Build<NameEntry>()
                   .With(ne => ne.State, State.PUBLISHED)
                   .With(ne => ne.CreatedBy, CreatedByIsmaila)
                   .With(ne => ne.Modified, (NameEntry?)default)
                   .With(ne => ne.Duplicates, [])
                   .Create()
        ];
    }
}