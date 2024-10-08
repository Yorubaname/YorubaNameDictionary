﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.StringObjectConverters
{
    public static class JsonSerializerOptionsProvider
    {
        public static JsonSerializerOptions GetJsonSerializerOptionsWithCustomConverters()
        {
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            options.Converters.Add(new CommaSeparatedStringConverter());
            options.Converters.Add(new HyphenSeparatedStringConverter());
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }
    }
}
