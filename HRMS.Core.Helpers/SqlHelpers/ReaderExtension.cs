using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Core.Helpers.SqlHelpers
{
    public static class ReaderExtension
    {
        public static T DefaultIfNull<T>(this SqlDataReader reader, string entityPropName)
        {
            if (!reader.IsDBNull(reader.GetOrdinal(entityPropName)))
                return (T)reader.GetValue(reader.GetOrdinal(entityPropName));
            return default(T);
        }
    }
}
