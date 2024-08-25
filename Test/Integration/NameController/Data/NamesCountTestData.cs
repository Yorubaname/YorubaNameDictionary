using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Core.Entities.NameEntry;
using Core.Enums;
namespace Test.Integration.NameController.Data;

public class NamesCountTestData : IEnumerable<object[]>
{
    private readonly IFixture _fixture;
    private readonly List<string> _nameSequence = new List<string> { "Ibironke", "Aderonke", "Olumide", "Akinwale", "Oluwaseun" };
    private int _nameIndex = 0;

    public NamesCountTestData()
    {
        _fixture = new Fixture();
        // Customize other properties if necessary
        _fixture.Customize<NameEntry>(c => c
            .With(ne => ne.State, State.PUBLISHED)
            .With(ne => ne.Modified, (NameEntry?)default)
            .With(ne => ne.Duplicates, [])
            .Do(ne => ne.Name = GetNextName())); // Generate names from a sequence
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { CreateNameEntries(5), 2 };
        yield return new object[] { CreateNameEntries(5), 4 };
        yield return new object[] { CreateNameEntries(5), 3 };
        yield return new object[] { CreateNameEntries(5), 5 };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private List<NameEntry> CreateNameEntries(int count)
    {
        // Create a list of NameEntries with the specified count
        return _fixture.CreateMany<NameEntry>(count).ToList();
    }

    private string GetNextName()
    {
        var name = _nameSequence[_nameIndex % _nameSequence.Count];
        _nameIndex++;
        return name;
    }
}