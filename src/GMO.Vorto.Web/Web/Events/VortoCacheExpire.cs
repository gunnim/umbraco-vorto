using Umbraco.Core.Composing;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;

namespace Our.Umbraco.Vorto.Web.Events
{
    public class SubscribeToDataTypeServiceSavingComponent : IComponent
    {
        // initialize: runs once when Umbraco starts
        public void Initialize()
        {
            DataTypeService.Saved += ExpireVortoCache;
        }

        // terminate: runs once when Umbraco stops
        public void Terminate()
        {
        }

        private void ExpireVortoCache(IDataTypeService sender, SaveEventArgs<IDataType> e)
        {
            foreach (var dataType in e.SavedEntities)
            {
                Current.AppCaches.RuntimeCache.Clear(
                    Constants.CacheKey_GetTargetDataTypeDefinition + dataType.Id);
            }
        }
    }
}
