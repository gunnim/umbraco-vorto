﻿using System;
using Newtonsoft.Json;

namespace Our.Umbraco.Vorto.Models
{
	class DataTypeInfo
	{
		[JsonProperty("guid")]
		public Guid Guid { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("propertyEditorAlias")]
		public string PropertyEditorAlias { get; set; }
	}
}
