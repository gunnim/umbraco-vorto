using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.PropertyEditors;

namespace GMO.Vorto.PropertyEditor
{
    public class VortoConfiguration
    {
        [ConfigurationField("dataType", "Data Type", "~/App_Plugins/Vorto/views/vorto.propertyEditorPicker.html", Description = "Select the data type to wrap.")]
        public string DataType { get; set; }

        [ConfigurationField("languageSource", "Language Source", "~/App_Plugins/Vorto/views/vorto.languageSourceRadioList.html", Description = "Select where Vorto should lookup the languages from.")]
        public string LanguageSource { get; set; }

        [ConfigurationField("xpath", "Language Nodes XPath", "textstring", Description = "If using in-use language source, enter an XPath statement to locate nodes containing language settings.")]
        public string XPath { get; set; }

        [ConfigurationField("displayNativeNames", "Display Native Language Names", "boolean", Description = "Set whether to display language names in their native form.")]
        public string DisplayNativeNames { get; set; }

        [ConfigurationField("primaryLanguage", "Primary Language", "~/App_Plugins/Vorto/views/vorto.languagePicker.html", Description = "Select the primary language for this field.")]
        public string PrimaryLanguage { get; set; }

        [ConfigurationField("mandatoryBehaviour", "Mandatory Field Behaviour", "~/App_Plugins/Vorto/views/vorto.mandatoryBehaviourPicker.html", Description = "Select how Vorto should handle mandatory fields.")]
        public string MandatoryBehaviour { get; set; }

        [ConfigurationField("rtlBehaviour", "RTL Behaviour", "~/App_Plugins/Vorto/views/vorto.rtlBehaviourPicker.html", Description = "[EXPERIMENTAL] Select how Vorto should handle Right-to-left languages. This feature is experimental so depending on the property being wrapped, results may vary.")]
        public string RtlBehaviour { get; set; }

        [ConfigurationField("hideLabel", "Hide Label", "boolean", Description = "Hide the Umbraco property title and description, making the Vorto span the entire page width")]
        public bool HideLabel { get; set; }
    }
}
