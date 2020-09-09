using ClientDependency.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Our.Umbraco.Vorto.Helpers;
using Our.Umbraco.Vorto.Models;
using Our.Umbraco.Vorto.PropertyEditor;
using System;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Editors;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web.PropertyEditors;

namespace Our.Umbraco.Vorto.Web.PropertyEditors
{
    [PropertyEditorAsset(ClientDependencyType.Javascript, "~/App_Plugins/Vorto/js/jquery.hoverIntent.minified.js", Priority = 1)]
    [PropertyEditorAsset(ClientDependencyType.Javascript, "~/App_Plugins/Vorto/js/vorto.js", Priority = 2)]
    [PropertyEditorAsset(ClientDependencyType.Css, "~/App_Plugins/Vorto/css/vorto.css", Priority = 2)]
    [DataEditor("Our.Umbraco.Vorto", "Vorto", "~/App_Plugins/Vorto/Views/vorto.html",
        ValueType = ValueTypes.Json)]
    public class VortoPropertyEditor : DataEditor
    {
        public VortoPropertyEditor(ILogger logger)
            : base(logger)
        {
        }

        /// <inheritdoc />
        protected override IConfigurationEditor CreateConfigurationEditor() => new VortoConfigurationEditor();

        #region Value Editor

        protected override IDataValueEditor CreateValueEditor()
        {
            return new VortoPropertyValueEditor(Attribute);
        }

        internal class VortoPropertyValueEditor : DataValueEditor
        {
            public VortoPropertyValueEditor(DataEditorAttribute attribute)
                : base(attribute)
            { }

            public override string ConvertDbToString(
                PropertyType propertyType,
                object value,
                IDataTypeService dataTypeService)
            {
                if (value == null || value.ToString().IsNullOrWhiteSpace())
                    return string.Empty;

                // Something weird is happening in core whereby ConvertDbToString is getting
                // called loads of times on publish, forcing the property value to get converted
                // again, which in tern screws up the values. To get round it, we create a 
                // dummy property copying the original properties value, this way not overwriting
                // the original property value allowing it to be re-converted again later

                // changed to temp var
                var res = value;

                try
                {
                    var deserializedValue = JsonConvert.DeserializeObject<VortoValue>(value.ToString());
                    if (deserializedValue.Values != null)
                    {
                        var dtd = VortoHelper.GetTargetDataTypeDefinition(deserializedValue.DtdGuid);
                        var propEditor = dtd.Editor;
                        var propType = new PropertyType(dtd);

                        var keys = deserializedValue.Values.Keys.ToArray();
                        foreach (var key in keys)
                        {
                            var newValue = propEditor.GetValueEditor().ConvertDbToString(
                                propType,
                                deserializedValue.Values[key] == null
                                    ? null
                                    : deserializedValue.Values[key].ToString(),
                                dataTypeService);

                            deserializedValue.Values[key] = newValue;
                        }

                        res = JsonConvert.SerializeObject(deserializedValue);
                    }
                }
                catch (Exception ex)
                {
                    Current.Logger.Error<VortoPropertyValueEditor>("Error converting DB value to String", ex);
                }

                return base.ConvertDbToString(propertyType, res, dataTypeService);
            }

            public override object ToEditor(Property property, IDataTypeService dataTypeService, string culture = null, string segment = null)
            {
                if (property.GetValue() == null || property.GetValue().ToString().IsNullOrWhiteSpace())
                    return string.Empty;

                // Something weird is happening in core whereby ConvertDbToString is getting
                // called loads of times on publish, forcing the property value to get converted
                // again, which in tern screws up the values. To get round it, we create a 
                // dummy property copying the original properties value, this way not overwriting
                // the original property value allowing it to be re-converted again later
                var prop2 = new Property(property.PropertyType);
                prop2.SetValue(property.GetValue());

                try
                {
                    var value = JsonConvert.DeserializeObject<VortoValue>(property.GetValue().ToString());
                    if (value.Values != null)
                    {
                        var dtd = VortoHelper.GetTargetDataTypeDefinition(value.DtdGuid);
                        var propEditor = dtd.Editor;
                        var propType = new PropertyType(dtd);

                        var keys = value.Values.Keys.ToArray();
                        foreach (var key in keys)
                        {
                            var prop = new Property(propType);
                            prop.SetValue(value?.Values[key]?.ToString());
                            var newValue = propEditor.GetValueEditor().ToEditor(
                                prop,
                                dataTypeService);
                            value.Values[key] = (newValue == null) ? null : JToken.FromObject(newValue);
                        }

                        prop2.SetValue(JsonConvert.SerializeObject(value));
                    }
                }
                catch (Exception ex)
                {
                    Current.Logger.Error<VortoPropertyValueEditor>("Error converting DB value to Editor", ex);
                }

                return base.ToEditor(prop2, dataTypeService, culture, segment);
            }

            public override object FromEditor(ContentPropertyData editorValue, object currentValue)
            {
                if (editorValue.Value == null || editorValue.Value.ToString().IsNullOrWhiteSpace())
                    return string.Empty;

                try
                {
                    var value = JsonConvert.DeserializeObject<VortoValue>(editorValue.Value.ToString());
                    if (value.Values != null)
                    {
                        var dtd = VortoHelper.GetTargetDataTypeDefinition(value.DtdGuid);

                        var propEditor = dtd.Editor;

                        var keys = value.Values.Keys.ToArray();
                        foreach (var key in keys)
                        {
                            var propData = new ContentPropertyData(value.Values[key], dtd.Configuration);
                            var newValue = propEditor.GetValueEditor().FromEditor(propData, value.Values[key]);
                            value.Values[key] = (newValue == null) ? null : JToken.FromObject(newValue);
                        }
                    }
                    return JsonConvert.SerializeObject(value);
                }
                catch (Exception ex)
                {
                    Current.Logger.Error<VortoPropertyValueEditor>("Error converting DB value to Editor", ex);
                }

                return base.FromEditor(editorValue, currentValue);
            }
        }

        #endregion
    }
}
