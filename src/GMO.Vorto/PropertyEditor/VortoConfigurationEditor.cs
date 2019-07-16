using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
