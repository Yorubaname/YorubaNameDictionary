using YorubaOrganization.Core.Dto.Response;
using YorubaOrganization.Core.Entities;

namespace Application.Mappers
{
    public static class GeoLocationHider
    {
        /// <summary>
        /// Returns copies of an entry's locations with <see cref="GeoLocationDto.HideOnWebsite"/> taken from the
        /// current locations list (matched on Place and Region, ignoring case). The copy an entry stores is a snapshot
        /// from when it was saved, so its own flag is ignored. A location with no match is not hidden.
        /// </summary>
        public static List<GeoLocationDto> Apply(List<GeoLocationDto> locations, IEnumerable<GeoLocation> current)
        {
            var hidden = current.Where(c => c.HideOnWebsite).ToList();

            return [.. locations.Select(l => l with
            {
                HideOnWebsite = hidden.Any(h =>
                    string.Equals(h.Place, l.Place, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(h.Region, l.Region, StringComparison.OrdinalIgnoreCase))
            })];
        }
    }
}
