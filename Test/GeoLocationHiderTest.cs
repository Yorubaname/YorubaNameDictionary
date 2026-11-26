using System.Collections.Generic;
using System.Linq;
using Application.Mappers;
using YorubaOrganization.Core.Dto.Response;
using YorubaOrganization.Core.Entities;
using Xunit;

namespace Test
{
    public class GeoLocationHiderTest
    {
        private static GeoLocation Current(string place, string region, bool hide) =>
            new(place, region) { HideOnWebsite = hide };

        [Fact]
        public void Apply_MarksLocationHiddenWhenCurrentListFlagsIt()
        {
            var result = GeoLocationHider.Apply(
                [new GeoLocationDto("1", "GENERAL", "GENERAL")],
                [Current("GENERAL", "GENERAL", true)]);

            Assert.True(Assert.Single(result).HideOnWebsite);
        }

        [Fact]
        public void Apply_IgnoresTheEmbeddedFlag()
        {
            var result = GeoLocationHider.Apply(
                [new GeoLocationDto("1", "IBADAN", "NWY", HideOnWebsite: true)],
                [Current("IBADAN", "NWY", false)]);

            Assert.False(Assert.Single(result).HideOnWebsite);
        }

        [Fact]
        public void Apply_MatchIsCaseInsensitive()
        {
            var result = GeoLocationHider.Apply(
                [new GeoLocationDto("1", "General", "general")],
                [Current("GENERAL", "GENERAL", true)]);

            Assert.True(Assert.Single(result).HideOnWebsite);
        }

        [Fact]
        public void Apply_NoMatchMeansNotHidden()
        {
            var result = GeoLocationHider.Apply(
                [new GeoLocationDto("1", "REMOVED", "NOWHERE")],
                [Current("GENERAL", "GENERAL", true)]);

            Assert.False(Assert.Single(result).HideOnWebsite);
        }

        [Fact]
        public void Apply_MatchesOnPlaceAndRegionTogether()
        {
            var result = GeoLocationHider.Apply(
                [new GeoLocationDto("1", "OWO", "OTHER")],
                [Current("OWO", "SEY", true)]);

            Assert.False(Assert.Single(result).HideOnWebsite);
        }

        [Fact]
        public void Apply_KeepsOrderAndOtherFields()
        {
            var result = GeoLocationHider.Apply(
                [new GeoLocationDto("1", "GENERAL", "GENERAL"), new GeoLocationDto("2", "IBADAN", "NWY")],
                [Current("GENERAL", "GENERAL", true), Current("IBADAN", "NWY", false)]);

            Assert.Equal(["GENERAL", "IBADAN"], result.Select(g => g.Place));
            Assert.Equal(["1", "2"], result.Select(g => g.Id));
            Assert.Equal([true, false], result.Select(g => g.HideOnWebsite));
        }
    }
}
