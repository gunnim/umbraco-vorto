using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using GMO.Vorto.PropertyEditor;
using Newtonsoft.Json;
using Our.Umbraco.Vorto.Exceptions;
using Our.Umbraco.Vorto.Models;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;

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
