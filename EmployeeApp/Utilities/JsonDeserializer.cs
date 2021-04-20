using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpClient.Utilities
{
    static class JsonDeserializer
    {
        /// <summary>
        /// Deserialize function to convert json to target object
        /// where json could be serialized to either of the passed types array
        /// </summary>
        /// <param name="json"></param>
        /// <param name="target"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static bool TryDeserialize(string json, out object target, params Type[] types)
        {
            foreach (Type type in types)
            {
                try
                {
                    target = JsonConvert.DeserializeObject(json, type);
                    return true;
                }
                catch (Exception)
                {
                }
            }
            target = null;
            return false;
        }
    }
}
