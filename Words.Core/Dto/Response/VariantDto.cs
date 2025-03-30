using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Words.Core.Dto.Response
{
    public record VariantDto(string Word, string? Geolocation)
    {
    }
}
