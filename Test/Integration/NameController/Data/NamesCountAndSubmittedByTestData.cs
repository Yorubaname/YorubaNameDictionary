using System.Collections;
using System.Collections.Generic;
using AutoFixture;
using Core.Entities.NameEntry;

namespace Test.Integration.NameController.Data
{
    public class NamesCountAndSubmittedByTestData : IEnumerable<object[]>
    {
        private readonly string[] _submittedBy = ["Adeshina", "Ismaila"];
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { CreateNameEntries(), 4, _submittedBy[0] };
            yield return new object[] { CreateNameEntries(), 5, _submittedBy[1] };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private List<NameEntry> CreateNameEntries()
        {
            var fixture = new Fixture();

            var nameEntry1 = fixture.Build<NameEntry>()
                .With(ne => ne.CreatedBy, _submittedBy[0])
                .With(ne => ne.Modified, (NameEntry?)default)
                .With(ne => ne.Duplicates, [])
                .Create();

            var nameEntry2 = fixture.Build<NameEntry>()
                .With(ne => ne.CreatedBy, _submittedBy[1])
                .With(ne => ne.Modified, (NameEntry?)default)
                .With(ne => ne.Duplicates, [])
                .Create();

            return new List<NameEntry> { nameEntry1, nameEntry2 };
        }
    }
}