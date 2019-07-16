using Newtonsoft.Json;
using Our.Umbraco.Vorto.Models;
using System;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Vorto.Converters
{
    public class VortoValueConverter : PropertyValueConverterBase
    {
        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
            => typeof(VortoValue);

        public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType)
            => PropertyCacheLevel.Element;

        public override bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Our.Umbraco.Vorto");
        }

        public override object ConvertIntermediateToObject(
            IPublishedElement owner,
            IPublishedPropertyType propertyType,
            PropertyCacheLevel cacheLevel,
            object source,
            bool preview)
        {
            try
            {
                if (source != null && !source.ToString().IsNullOrWhiteSpace())
                {
                    return JsonConvert.DeserializeObject<VortoValue>(source.ToString());
                }
            }
            catch (Exception e)
            {
                Current.Logger.Error<VortoValueConverter>(e, "Error converting Vorto value");
            }

            return null;
        }
    }
}
