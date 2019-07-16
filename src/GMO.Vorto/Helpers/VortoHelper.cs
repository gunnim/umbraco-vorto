using GMO.Vorto.PropertyEditor;
using Our.Umbraco.Vorto.Exceptions;
using System;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;

namespace Our.Umbraco.Vorto.Helpers
{
    internal static class VortoHelper
    {
        internal static IDataType GetTargetDataTypeDefinition(Guid myId)
        {
            return (IDataType)Current.AppCaches.RuntimeCache.Get(
                Constants.CacheKey_GetTargetDataTypeDefinition + myId,
                () =>
                {
                    // Get instance of our own datatype so we can lookup the actual datatype from prevalue
                    var services = Current.Services;
                    var dtd = services.DataTypeService.GetDataType(myId);

                    if (dtd != null)
                    {
                        VortoConfiguration valueList = (VortoConfiguration)dtd.Configuration;

                        return services.DataTypeService.GetDataType(valueList.DataType.Guid);
                    }

                    throw new VortoException($"Unable to get Vorto data type using id {myId} !");
                });
        }
    }
}
