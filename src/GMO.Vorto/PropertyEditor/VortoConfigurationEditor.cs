using System.Collections.Generic;
using Umbraco.Core.PropertyEditors;

namespace GMO.Vorto.PropertyEditor
{
    class VortoConfigurationEditor : ConfigurationEditor<VortoConfiguration>
    {
        public override IDictionary<string, object> DefaultConfiguration => new Dictionary<string, object>
        {
                {"languageSource", "installed"},
                {"mandatoryBehaviour", "ignore"},
                {"rtlBehaviour", "ignore"},
        };
    }
}
