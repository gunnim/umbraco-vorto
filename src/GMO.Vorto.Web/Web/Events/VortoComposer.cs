using Our.Umbraco.Vorto.Web.Events;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace GMO.Vorto.Web.Web.Events
{
    public class VortoComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Components().Append<SubscribeToDataTypeServiceSavingComponent>();
            composition.Components().Append<VortoServerVariablesParser>();
        }
    }
}
