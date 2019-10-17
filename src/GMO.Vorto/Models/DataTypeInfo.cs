using Newtonsoft.Json;
using System;

namespace Our.Umbraco.Vorto.Models
{
    public class DataTypeInfo
    {
        [JsonProperty("guid")]
        public Guid Guid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("propertyEditorAlias")]
        public string PropertyEditorAlias { get; set; }
    }
}
